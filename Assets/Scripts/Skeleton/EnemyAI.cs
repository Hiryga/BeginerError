using UnityEngine;
using UnityEngine.AI;
using BeginerError.Utils;
using System;

public class EnemyAI : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;

        // Зона обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        // Зона атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackingDistance);
    }

    [Header("States")]
    [SerializeField] private State startingState = State.Roaming;
    [SerializeField] private bool isAttackingEnemy = true;

    [Header("Roaming")]
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;

    [Header("Detection & Chasing")]
    [SerializeField] private float detectionDistance = 15f;      // Радиус обнаружения игрока при патрулировании
    [SerializeField] private float chasingSpeedMultiplier = 2f;
    [SerializeField] private float chaseTimeout = 3f;           // Как долго преследовать после провокации

    [Header("Attacking")]
    [SerializeField] private float attackingDistance = 2f;
    [SerializeField] private float attackRate = 2f;

    private float _nextAttackTime = 0f;

    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    private float _roamingTimer;
    private Vector3 _roamPosition;
    private Vector3 _startingPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _nextCheckDirectionTime = 0f;
    private readonly float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    // Таймер преследования: когда > 0, враг активно преследует
    private float _chaseTimer = 0f;

    // Флаг активного преследования: включается когда видим игрока или провоцированы
    private bool _isActivelyChasing = false;

    public event EventHandler OnEnemyAttack;

    public bool IsRunning => _navMeshAgent.velocity.magnitude > 0.01f;

    public float AttackingDistance => attackingDistance;

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = startingState;

        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * chasingSpeedMultiplier;
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();
    }

    public void SetDeathState()
    {
        _navMeshAgent.ResetPath();
        _currentState = State.Death;
    }

    /// <summary>
    /// Вызывается EnemyEntity при получении урона.
    /// Провоцирует врага на преследование.
    /// </summary>
    public void Provoke()
    {
        if (_currentState == State.Death) return;

        _isActivelyChasing = true;
        _chaseTimer = chaseTimeout;
    }

    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Roaming:
                HandleRoaming();
                CheckCurrentState(); // Проверяем КАЖДЫЙ кадр в Roaming
                break;

            case State.Chasing:
                ChasingTarget();
                HandleChaseTimeout();
                CheckCurrentState();
                break;

            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;

            case State.Death:
                break;

            default:
            case State.Idle:
                CheckCurrentState();
                break;
        }
    }


    private void HandleRoaming()
    {
        _roamingTimer -= Time.deltaTime;
        if (_roamingTimer <= 0)
        {
            Roaming();
            _roamingTimer = roamingTimerMax;
        }
    }

    private void HandleChaseTimeout()
    {
        if (!_isActivelyChasing) return;

        _chaseTimer -= Time.deltaTime;
        if (_chaseTimer <= 0f)
        {
            // Время преследования истекло
            _isActivelyChasing = false;
            _chaseTimer = 0f;
        }
    }

    private void ChasingTarget()
    {
        if (Player.Instance == null) return;
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    public float GetRoamingAnimationSpeed()
    {
        return _navMeshAgent.speed / _roamingSpeed;
    }

    private void CheckCurrentState()
    {
        if (Player.Instance == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        bool playerIsAlive = Player.Instance.IsAlive();
        State newState = _currentState;

        // ====== ЛОГИКА ОПРЕДЕЛЕНИЯ СОСТОЯНИЯ ======

        // 1️⃣ АТАКА - самый высокий приоритет
        if (isAttackingEnemy && distanceToPlayer <= attackingDistance && playerIsAlive)
        {
            newState = State.Attacking;
        }
        // 2️⃣ ПРЕСЛЕДОВАНИЕ - если мы активно преследуем ИЛИ видим игрока в зоне обнаружения
        else if (playerIsAlive && (
            _isActivelyChasing ||  // Включено флагом провокации
            distanceToPlayer <= detectionDistance  // ИЛИ видим в зоне обнаружения
        ))
        {
            newState = State.Chasing;

            // Если видим игрока при патрулировании - включаем преследование
            if (!_isActivelyChasing && distanceToPlayer <= detectionDistance)
            {
                _isActivelyChasing = true;
                _chaseTimer = chaseTimeout;
                Debug.Log($"[{gameObject.name}] Видим игрока! Начинаем преследование");
            }
        }
        // 3️⃣ ПАТРУЛИРОВАНИЕ - дефолт состояние
        else
        {
            newState = State.Roaming;
            _isActivelyChasing = false;
            _chaseTimer = 0f;
        }

        // ====== ПРИМЕНЕНИЕ СМЕНЫ СОСТОЯНИЯ ======
        if (newState != _currentState)
        {
            Debug.Log($"[{gameObject.name}] Переход: {_currentState} → {newState}");

            if (newState == State.Chasing)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                _roamingTimer = 0f;
                _navMeshAgent.speed = _roamingSpeed;
                Roaming();
            }
            else if (newState == State.Attacking)
            {
                _navMeshAgent.ResetPath();
            }

            _currentState = newState;
        }
    }


    private void AttackingTarget()
    {
        if (Player.Instance == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        // Если игрок убежал из зоны атаки - не атакуем, но можем преследовать дальше
        if (distanceToPlayer > attackingDistance && _isActivelyChasing)
        {
            // Сохраняем преследование, выходим из Attacking в следующем CheckCurrentState
            return;
        }

        // Атакуем только если игрок в зоне атаки
        if (Time.time >= _nextAttackTime && distanceToPlayer <= attackingDistance)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            _nextAttackTime = Time.time + attackRate;
        }

        // Смотрим на игрока
        if (Player.Instance != null)
        {
            ChangeFacingDirection(transform.position, Player.Instance.transform.position);
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time < _nextCheckDirectionTime) return;

        if (IsRunning)
        {
            ChangeFacingDirection(_lastPosition, transform.position);
        }
        else if (_currentState == State.Attacking && Player.Instance != null)
        {
            ChangeFacingDirection(transform.position, Player.Instance.transform.position);
        }

        _lastPosition = transform.position;
        _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
    }

    private void Roaming()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        transform.rotation = sourcePosition.x > targetPosition.x
            ? Quaternion.Euler(0, -180, 0)
            : Quaternion.Euler(0, 0, 0);
    }
}
