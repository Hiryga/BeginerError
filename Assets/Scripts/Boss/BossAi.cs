using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    public event EventHandler OnBossAttack;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 5f;
    [SerializeField] private float attackChargeTime = 2f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float attackRadius = 2f;

    [Header("Stun (реакция на урон)")]
    [SerializeField] private float stunDuration = 0.5f;

    [Header("References")]
    [SerializeField] private GameObject attackIndicatorPrefab;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Rigidbody2D rigidbody2D;
    private BossEntity bossEntity;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isStunned = false;
    private float nextAttackTime = 0f;
    private Coroutine currentAttackCoroutine;
    private readonly List<GameObject> activeIndicators = new List<GameObject>();

    // поворот
    private float _nextCheckDirectionTime = 0f;
    private readonly float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    // фиксация направления после появления точки атаки
    private bool lockFacingDuringAttack = false;
    private Vector3 attackLookTarget;

    public bool IsRunning =>
        navMeshAgent != null &&
        navMeshAgent.enabled &&
        navMeshAgent.velocity.sqrMagnitude > 0.01f;

    public float GetRoamingAnimationSpeed()
    {
        if (navMeshAgent == null || moveSpeed <= 0.01f) return 1f;
        return navMeshAgent.speed / moveSpeed;
    }

    public bool IsFacingLocked => lockFacingDuringAttack;
    public Vector3 GetAttackLookTarget() => attackLookTarget;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        bossEntity = GetComponent<BossEntity>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[BossAI] Player с тегом 'Player' не найден!");

        if (bossEntity != null)
            bossEntity.OnDeath += BossEntity_OnDeath;

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

        // Движение: всегда идём к игроку, если НЕ атакуем и НЕ в стане
        if (!isAttacking && !isStunned && navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else if ((isAttacking || isStunned) && navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.ResetPath();
        }

        // Атака: только если не в стане и не атакуем
        if (!isStunned &&
            !isAttacking &&
            distanceToPlayer <= attackRange &&
            Time.time >= nextAttackTime)
        {
            currentAttackCoroutine = StartCoroutine(PerformAttack());
        }

        HandleFacingDirection();
    }

    private void HandleFacingDirection()
    {
        if (Time.time < _nextCheckDirectionTime) return;

        if (lockFacingDuringAttack)
        {
            // после появления точки атаки смотрим ТОЛЬКО в зафиксированную сторону
            ChangeFacingDirection(transform.position, attackLookTarget);
        }
        else
        {
            if (IsRunning)
            {
                ChangeFacingDirection(_lastPosition, transform.position);
            }
            else if (player != null && !isDead)
            {
                ChangeFacingDirection(transform.position, player.position);
            }
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

        GameObject indicator = null;

        // МОМЕНТ ПОЯВЛЕНИЯ ТОЧКИ АТАКИ
        if (attackIndicatorPrefab != null)
        {
            indicator = Instantiate(attackIndicatorPrefab, targetAttackPosition, Quaternion.identity);
            activeIndicators.Add(indicator);

            if (indicator.TryGetComponent(out AttackIndicator indicatorScript))
                indicatorScript.Initialize(attackChargeTime, attackRadius);

            if (player != null)
                attackLookTarget = player.position;   // фиксируем направление

            lockFacingDuringAttack = true;            // С ЭТОГО МОМЕНТА НЕ МЕНЯЕМ СТОРОНУ
        }

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
            lockFacingDuringAttack = false;
            isAttacking = false;
            yield break;
        }

        if (indicator != null)
        {
            Destroy(indicator);
            activeIndicators.Remove(indicator);
        }

        DamagePlayersInRadius(targetAttackPosition, attackRadius, attackDamage);

        lockFacingDuringAttack = false;   // после удара снова можно поворачиваться
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
            if (indicator != null) Destroy(indicator);

        activeIndicators.Clear();
    }

    public void SetDeathState()
    {
        if (isDead) return;

        isDead = true;
        lockFacingDuringAttack = false;
        isStunned = false;

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

    /// <summary>
    /// Вызывай из BossEntity.TakeDamage, если хочешь стан.
    /// </summary>
    public void Stun()
    {
        if (isDead) return;
        StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;

        if (navMeshAgent != null && navMeshAgent.enabled)
            navMeshAgent.ResetPath();

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
