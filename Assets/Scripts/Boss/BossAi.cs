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

    [Header("References")]
    [SerializeField] private GameObject attackIndicatorPrefab;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Rigidbody2D rigidbody2D;
    private BossEntity bossEntity;
    private bool isDead = false;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;
    private Coroutine currentAttackCoroutine;
    private List<GameObject> activeIndicators = new List<GameObject>(); // Список активных индикаторов

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        bossEntity = GetComponent<BossEntity>();

        Debug.Log("[BossAI] Awake - Компоненты инициализированы");
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
            Debug.LogError("[BossAI] ОШИБКА: Игрок не найден! Проверьте тег 'Player'");
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
            navMeshAgent.stoppingDistance = attackRange - 0.5f;
            Debug.Log($"[BossAI] NavMeshAgent готов. Speed: {moveSpeed}");
        }

        if (rigidbody2D != null)
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnDestroy()
    {
        if (bossEntity != null)
        {
            bossEntity.OnDeath -= BossEntity_OnDeath;
        }

        // Уничтожаем оставшиеся индикаторы
        DestroyAllIndicators();
    }

    private void Update()
    {
        if (isDead || player == null)
        {
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.ResetPath();
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange &&
            Time.time >= nextAttackTime &&
            !isAttacking)
        {
            Debug.Log("[BossAI] Начало атаки!");
            currentAttackCoroutine = StartCoroutine(PerformAttack());
        }
        else if (!isAttacking && navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        Debug.Log("[BossAI] ===== НАЧАЛО АТАКИ =====");

        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.ResetPath();
        }

        Vector3 targetAttackPosition = player.position;
        Debug.Log($"[BossAI] Цель атаки: {targetAttackPosition}");

        GameObject indicator = null;
        if (attackIndicatorPrefab != null)
        {
            indicator = Instantiate(attackIndicatorPrefab, targetAttackPosition, Quaternion.identity);
            activeIndicators.Add(indicator); // Добавляем в список

            AttackIndicator indicatorScript = indicator.GetComponent<AttackIndicator>();
            if (indicatorScript != null)
            {
                indicatorScript.Initialize(attackChargeTime, attackRadius);
                Debug.Log("[BossAI] Индикатор создан на позиции игрока");
            }
        }

        OnBossAttack?.Invoke(this, EventArgs.Empty);

        float timeElapsed = 0f;
        while (timeElapsed < attackChargeTime && !isDead)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // ВАЖНО: Если босс умер - отменяем атаку и уничтожаем индикатор
        if (isDead)
        {
            Debug.Log("[BossAI] ☠️ Босс умер! Атака отменена.");
            if (indicator != null)
            {
                Destroy(indicator);
                activeIndicators.Remove(indicator); // Удаляем из списка
            }
            isAttacking = false;
            yield break;
        }

        if (indicator != null)
        {
            Destroy(indicator);
            activeIndicators.Remove(indicator); // Удаляем из списка
        }

        // Наносим урон только если босс живой
        if (!isDead)
        {
            DamagePlayersInRadius(targetAttackPosition, attackRadius, attackDamage);
            Debug.Log($"[BossAI] ===== АТАКА ЗАВЕРШЕНА =====");
        }

        isAttacking = false;
    }

    private void DamagePlayersInRadius(Vector3 position, float radius, int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);
        System.Collections.Generic.HashSet<PlayerHealth> damagedPlayers =
            new System.Collections.Generic.HashSet<PlayerHealth>();

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null && !damagedPlayers.Contains(playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                    damagedPlayers.Add(playerHealth);
                    Debug.Log($"[BossAI] ⚔️ Урон нанесен игроку: {damage}");
                }
            }
        }

        if (damagedPlayers.Count == 0)
        {
            Debug.Log("[BossAI] ✓ Игрок уклонился от атаки!");
        }
    }

    // Уничтожаем ВСЕ активные индикаторы
    private void DestroyAllIndicators()
    {
        foreach (GameObject indicator in activeIndicators)
        {
            if (indicator != null)
            {
                Destroy(indicator);
                Debug.Log("[BossAI] ⛔ Индикатор уничтожен при смерти босса");
            }
        }
        activeIndicators.Clear();
    }

    public void SetDeathState()
    {
        isDead = true;

        // Останавливаем текущую атаку
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
            Debug.Log("[BossAI] ⛔ Текущая атака прервана");
        }

        // Уничтожаем ВСЕ индикаторы
        DestroyAllIndicators();

        isAttacking = false;

        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }
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
