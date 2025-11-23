using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private int damageAmount = 2;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Bow playerBow; // Ссылка на компонент Bow игрока (добавить в инспекторе)
    private PolygonCollider2D _polygonCollider2D;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    public void AttackColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }

    public void AttackColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_polygonCollider2D) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        bool hit = false;

        // Обычный враг
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
            Debug.Log($"[Sword] ⚔️ Удар по врагу! Урон: {damageAmount}");
            hit = true;
        }

        // БОСС
        if (collision.TryGetComponent(out BossEntity bossEntity))
        {
            bossEntity.TakeDamage(damageAmount);
            Debug.Log($"[Sword] 💢 УДАР ПО БОССУ! Урон: {damageAmount}");
            hit = true;
        }

        if (hit)
        {
            lastAttackTime = Time.time;
            // Даем 1 стрелу за удар
            if (playerBow != null)
                playerBow.AddArrows(1);
        }
    }
}
