using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private int damageAmount = 2;
    private PolygonCollider2D _polygonCollider2D;

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
        if (!_polygonCollider2D.enabled) return; // Фикс на случай ошибочного вызова
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
        }
    }

}
