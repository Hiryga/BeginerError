// PlayerAttackReceiver.cs
using UnityEngine;

public class PlayerAttackReceiver : MonoBehaviour
{
    private void OnEnable()
    {
        // Подписываемся на ВСЕХ врагов
        var enemies = FindObjectsOfType<EnemyAI>();
        foreach (var enemy in enemies)
        {
            enemy.OnEnemyAttack += HandleEnemyAttack;
        }
    }

    private void OnDisable()
    {
        var enemies = FindObjectsOfType<EnemyAI>();
        foreach (var enemy in enemies)
        {
            enemy.OnEnemyAttack -= HandleEnemyAttack;
        }
    }

    private void HandleEnemyAttack(object sender, System.EventArgs e)
    {
        // Проверяем дистанцию (чтобы не бить через стену)
        EnemyAI enemy = sender as EnemyAI;
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        if (distance <= enemy.AttackingDistance)
        {
            Player.Instance.TakeDamage(1); // или enemy.damage
        }
    }
}