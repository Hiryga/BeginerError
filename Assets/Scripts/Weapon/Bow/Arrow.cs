using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector2 flyDirection;
    private float flySpeed;
    [SerializeField] private int damageAmount = 2;
    [SerializeField] private float maxDistance = 10f;  // Maximum travel distance

    private Rigidbody2D rb;
    private bool hasHit = false;
    private Vector2 startPosition;

    public void Initialize(Vector2 direction, float speed)
    {
        flyDirection = direction.normalized;
        flySpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;  // Record starting position
        float angle = Mathf.Atan2(flyDirection.y, flyDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = flyDirection * flySpeed;

        // Check distance traveled
        if (Vector2.Distance(startPosition, rb.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
            Debug.Log($"[Arrow] 🎯 Hit enemy! Damage: {damageAmount}");
            hasHit = true;
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent(out BossEntity bossEntity))
        {
            bossEntity.TakeDamage(damageAmount);
            Debug.Log($"[Arrow] 💥 Hit boss! Damage: {damageAmount}");
            hasHit = true;
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Wall"))
        {
            Debug.Log("[Arrow] Arrow hit the wall");
            Destroy(gameObject);
        }
    }
}
