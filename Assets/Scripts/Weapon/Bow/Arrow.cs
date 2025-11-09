using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector2 flyDirection;
    private float flySpeed;
    [SerializeField] private int damageAmount = 2; // ← Используется!

    private Rigidbody2D rb;

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
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount); // ← Именно здесь урон применяется!
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
