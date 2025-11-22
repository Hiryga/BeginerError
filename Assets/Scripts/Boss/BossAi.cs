using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    public event EventHandler OnBossAttack;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f; // Радиус когда босс начинает атаку
    [SerializeField] private float attackCooldown = 5f; // Кулдаун между атаками
    [SerializeField] private float attackChargeTime = 2f; // Время каста (заполнение круга)
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float attackRadius = 2f; // Радиус урона

    [Header("References")]
    [SerializeField] private GameObject attackIndicatorPrefab;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Rigidbody2D rigidbody2D;
    private BossEntity bossEntity;
    private bool isDead = false;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

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

        // Если в радиусе атаки, готов атаковать и не атакует сейчас
        if (distanceToPlayer <= attackRange &&
            Time.time >= nextAttackTime &&
            !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
        // Иначе двигаемся к игроку
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

        // Остановить движение
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.ResetPath();
        }

        // ВАЖНО: Запоминаем позицию игрока В МОМЕНТ НАЧАЛА КАСТА
        Vector3 targetAttackPosition = player.position;

        Debug.Log($"[BossAI] Цель атаки: {targetAttackPosition}");

        // Создать индикатор атаки НА ПОЗИЦИИ ИГРОКА
        GameObject indicator = null;
        if (attackIndicatorPrefab != null)
        {
            indicator = Instantiate(attackIndicatorPrefab, targetAttackPosition, Quaternion.identity);
            AttackIndicator indicatorScript = indicator.GetComponent<AttackIndicator>();
            if (indicatorScript != null)
            {
                indicatorScript.Initialize(attackChargeTime, attackRadius);
                Debug.Log("[BossAI] Индикатор создан на позиции игрока");
            }
            else
            {
                Debug.LogError("[BossAI] AttackIndicator скрипт не найден!");
            }
        }
        else
        {
            Debug.LogError("[BossAI] Attack Indicator Prefab не назначен!");
        }

        // Событие для анимации
        OnBossAttack?.Invoke(this, EventArgs.Empty);

        // Ждем пока круг заполняется
        Debug.Log($"[BossAI] Каст атаки... ({attackChargeTime} сек)");
        yield return new WaitForSeconds(attackChargeTime);

        // Уничтожить индикатор
        if (indicator != null)
        {
            Destroy(indicator);
        }

        // Наносим урон по сохраненной позиции (где был игрок в начале каста)
        DamagePlayersInRadius(targetAttackPosition, attackRadius, attackDamage);

        Debug.Log($"[BossAI] ===== АТАКА ЗАВЕРШЕНА ===== (следующая через {attackCooldown} сек)");

        isAttacking = false;
    }

    private void DamagePlayersInRadius(Vector3 position, float radius, int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);

        // Используем HashSet чтобы не дамажить одного игрока несколько раз
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

    public void SetDeathState()
    {
        isDead = true;
        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;
        }
    }

    private void BossEntity_OnDeath(object sender, EventArgs e)
    {
        SetDeathState();
    }

    private void OnDrawGizmosSelected()
    {
        // Красный круг - радиус начала атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Желтый круг - радиус урона
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
