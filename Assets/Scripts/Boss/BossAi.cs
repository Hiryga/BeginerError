using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    public event EventHandler OnBossAttack;   // триггер анимации атаки

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float chaseDistance = 10f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 5f;
    [SerializeField] private float attackChargeTime = 2f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float attackRadius = 2f;

    [Header("References")]
    [SerializeField] private GameObject attackIndicatorPrefab;


    private float _nextCheckDirectionTime = 0f;
    private readonly float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Rigidbody2D rigidbody2D;
    private BossEntity bossEntity;

    private bool isDead = false;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;
    private Coroutine currentAttackCoroutine;
    private readonly List<GameObject> activeIndicators = new List<GameObject>();

    public bool IsRunning => navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.velocity.sqrMagnitude > 0.01f;

    public float GetRoamingAnimationSpeed()
    {
        if (navMeshAgent == null || moveSpeed <= 0.01f) return 1f;
        return navMeshAgent.speed / moveSpeed;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        bossEntity = GetComponent<BossEntity>();

        Debug.Log("[BossAI] Awake - компоненты инициализированы");
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("[BossAI] Игрок найден: " + player.gameObject.name);
        }
        else
        {
            Debug.LogError("[BossAI] Игрок с тегом 'Player' не найден!");
        }

        if (bossEntity != null)
        {
            bossEntity.OnDeath += BossEntity_OnDeath;
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.stoppingDistance = attackRange - 0.3f;
        }

        if (rigidbody2D != null)
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnDestroy()
    {
        if (bossEntity != null)
            bossEntity.OnDeath -= BossEntity_OnDeath;

        DestroyAllIndicators();
    }

    private void Update()
    {
        if (isDead || player == null)
        {
            if (navMeshAgent != null && navMeshAgent.enabled)
                navMeshAgent.ResetPath();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ВСЕГДА идём за игроком, если не кастуем атаку
        if (!isAttacking && navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
        }

        // Проверка на атаку
        if (distanceToPlayer <= attackRange &&
            Time.time >= nextAttackTime &&
            !isAttacking)
        {
            currentAttackCoroutine = StartCoroutine(PerformAttack());
        }

        HandleFacingDirection();
    }

    private void HandleFacingDirection()
    {
        if (Time.time < _nextCheckDirectionTime) return;

        // Если босс движется – ориентируемся по смещению
        if (IsRunning)
        {
            ChangeFacingDirection(_lastPosition, transform.position);
        }
        else if (player != null && (isAttacking || !isDead))
        {
            // Если стоим (например, кастуем) – смотрим на игрока
            ChangeFacingDirection(transform.position, player.position);
        }

        _lastPosition = transform.position;
        _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
            transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }


    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        if (navMeshAgent != null && navMeshAgent.enabled)
            navMeshAgent.ResetPath();

        Vector3 targetAttackPosition = player != null ? player.position : transform.position;

        // создаём индикатор
        GameObject indicator = null;
        if (attackIndicatorPrefab != null)
        {
            indicator = Instantiate(attackIndicatorPrefab, targetAttackPosition, Quaternion.identity);
            activeIndicators.Add(indicator);

            AttackIndicator indicatorScript = indicator.GetComponent<AttackIndicator>();
            if (indicatorScript != null)
                indicatorScript.Initialize(attackChargeTime, attackRadius);
        }

        // говорим аниматору начать анимацию удара
        OnBossAttack?.Invoke(this, EventArgs.Empty);

        float timeElapsed = 0f;
        while (timeElapsed < attackChargeTime && !isDead)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (isDead)
        {
            if (indicator != null)
            {
                Destroy(indicator);
                activeIndicators.Remove(indicator);
            }
            isAttacking = false;
            yield break;
        }

        if (indicator != null)
        {
            Destroy(indicator);
            activeIndicators.Remove(indicator);
        }

        // наносим урон
        DamagePlayersInRadius(targetAttackPosition, attackRadius, attackDamage);

        isAttacking = false;
    }

    private void DamagePlayersInRadius(Vector3 position, float radius, int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);
        var damagedPlayers = new HashSet<PlayerHealth>();

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            if (hit.TryGetComponent(out PlayerHealth playerHealth) &&
                !damagedPlayers.Contains(playerHealth))
            {
                playerHealth.TakeDamage(damage);
                damagedPlayers.Add(playerHealth);
            }
        }
    }

    private void DestroyAllIndicators()
    {
        foreach (GameObject indicator in activeIndicators)
        {
            if (indicator != null)
                Destroy(indicator);
        }
        activeIndicators.Clear();
    }

    public void SetDeathState()
    {
        if (isDead) return;

        isDead = true;

        if (currentAttackCoroutine != null)
            StopCoroutine(currentAttackCoroutine);

        DestroyAllIndicators();
        isAttacking = false;

        if (navMeshAgent != null && navMeshAgent.enabled)
            navMeshAgent.enabled = false;
    }

    private void BossEntity_OnDeath(object sender, EventArgs e)
    {
        SetDeathState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
