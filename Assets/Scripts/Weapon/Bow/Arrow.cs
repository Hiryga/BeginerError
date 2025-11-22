using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector2 flyDirection;
    private float flySpeed;
    [SerializeField] private int damageAmount = 2;

    private Rigidbody2D rb;
    private bool hasHit = false; // Предотвращаем множественные попадания

    public void Initialize(Vector2 direction, float speed)
    {
        flyDirection = direction.normalized;
        flySpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        float angle = Mathf.Atan2(flyDirection.y, flyDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = flyDirection * flySpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // Уже попала в кого-то

        // ПРОВЕРКА 1: Обычный враг (EnemyEntity)
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
            Debug.Log($"[Arrow] 🎯 Попадание по врагу! Урон: {damageAmount}");
            hasHit = true;
            Destroy(gameObject);
            return;
        }

        // ПРОВЕРКА 2: БОСС (BossEntity)
        if (collision.TryGetComponent(out BossEntity bossEntity))
        {
            bossEntity.TakeDamage(damageAmount);
            Debug.Log($"[Arrow] 💥 ПОПАДАНИЕ ПО БОССУ! Урон: {damageAmount}");
            hasHit = true;
            Destroy(gameObject);
            return;
        }

        // Столкновение со стеной
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("[Arrow] Стрела попала в стену");
            Destroy(gameObject);
        }
    }
}
