using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [SerializeField] private BossEntity bossEntity;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackCooldown = 3f;

    private float attackTimer;

    void Update()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist > attackRange)
        {
            // Идём к игроку (медленно)
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            // Здесь можно добавить анимацию движения
        }
        else
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0;
                StartCoroutine(AttackSequence());
            }
        }
    }

    private IEnumerator AttackSequence()
    {
        // Механика замаха (красный круг — эффект на земле)
        // Пример: показать круг, заполнить его, нанести урон
        Vector2 attackPos = playerTransform.position;
        yield return StartCoroutine(ShowAttackWarning(attackPos, 1.2f)); // 1.2 сек на замах
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            // Здесь вызывай урон игроку, например:
            // playerHealth.TakeDamage(5);
        }
    }

    private IEnumerator ShowAttackWarning(Vector2 pos, float chargeTime)
    {
        // За этот chargeTime визуал круга постепенно заполняется
        // Реализуй визуализацию: например, через SpriteRenderer или UI Image
        yield return new WaitForSeconds(chargeTime);
    }
}
