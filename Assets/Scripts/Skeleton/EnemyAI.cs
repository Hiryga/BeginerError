using UnityEngine;
using UnityEngine.AI;
using BeginerError.Utils;
using System;

public class EnemyAI : MonoBehaviour
{
    [Header("States")]
    [SerializeField] private State startingState = State.Roaming;
    [SerializeField] private bool isChasingEnemy = false;
    [SerializeField] private bool isAttackingEnemy = true;

    [Header("Roaming")]
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamimgDistanceMin = 3f;
    [SerializeField] private float roamimgTimerMax = 2f;

    [Header("Chasing")]
    [SerializeField] private float chasingDistance = 4f;
    [SerializeField] private float chasingSpeedMultiplier = 2f;
    [SerializeField] private float chaseTimeout = 3f;   // сколько секунд преследует после провокации

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

    // таймер преследования после провокации
    private float _chaseTimer = 0f;
    private bool _wasProvoked = false;

    public event EventHandler OnEnemyAttack;

    public bool IsRunning => _navMeshAgent.velocity != Vector3.zero;

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
    /// Переводит врага в состояние преследования.
    /// </summary>
    public void Provoke()
    {
        if (_currentState == State.Death) return;

        _wasProvoked = true;
        _chaseTimer = chaseTimeout;
        isChasingEnemy = true;

        _navMeshAgent.speed = _chasingSpeed;
        _currentState = State.Chasing;
    }

    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Roaming:
                _roamingTimer -= Time.deltaTime;
                if (_roamingTimer < 0)
                {
                    Roaming();
                    _roamingTimer = roamimgTimerMax;
                }
                CheckCurrentState();
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
                // ничего не делаем
                break;

            default:
            case State.Idle:
                CheckCurrentState();
                break;
        }
    }

    private void HandleChaseTimeout()
    {
        if (!_wasProvoked) return;

        _chaseTimer -= Time.deltaTime;
        if (_chaseTimer <= 0f)
        {
            // перестаём преследовать и возвращаемся к бродяжничеству
            _wasProvoked = false;
            isChasingEnemy = false;
            _navMeshAgent.speed = _roamingSpeed;
            _roamingTimer = 0f;
            _currentState = State.Roaming;
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
        State newState = _currentState;

        // логика преследования по дистанции (если включено isChasingEnemy)
        if (isChasingEnemy && _currentState != State.Death)
        {
            if (distanceToPlayer <= chasingDistance)
                newState = State.Chasing;
        }

        // логика атаки
        if (isAttackingEnemy && _currentState != State.Death)
        {
            if (distanceToPlayer <= attackingDistance)
                newState = Player.Instance.IsAlive() ? State.Attacking : State.Roaming;
        }

        // если игрок слишком далеко, а провокация уже прошла — уходим в Roaming
        if (!_wasProvoked && distanceToPlayer > chasingDistance * 2f && _currentState == State.Chasing)
        {
            newState = State.Roaming;
        }

        if (newState != _currentState)
        {
            if (newState == State.Chasing)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                _roamingTimer = 0f;
                _navMeshAgent.speed = _roamingSpeed;
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
        if (Time.time > _nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            _nextAttackTime = Time.time + attackRate;
        }

        // в атаке тоже смотрим на игрока
        if (Player.Instance != null)
        {
            ChangeFacingDirection(transform.position, Player.Instance.transform.position);
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time > _nextCheckDirectionTime)
        {
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
    }

    private void Roaming()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamimgDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        transform.rotation = sourcePosition.x > targetPosition.x
            ? Quaternion.Euler(0, -180, 0)
            : Quaternion.Euler(0, 0, 0);
    }
}
