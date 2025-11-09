using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private bool _playerHitThisAttack = false;

    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;

    [SerializeField] private int _maxHealth;
    private int _currentHealth;

    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private EnemyAI _enemyAI;
    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }
    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        // ВЫВОД В КОНСОЛЬ
        Debug.Log($"[Enemy] {gameObject.name} HP: {_currentHealth}/{_maxHealth}");

        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }

    public void PolygonColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
        _playerHitThisAttack = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_polygonCollider2D.enabled && !_playerHitThisAttack && collision.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(1); // или свой параметр урона
            _playerHitThisAttack = true; // игрок НЕ получит урон до следующей атаки
        }
    }


    private void DetectDeath()
    {
        if (_currentHealth <= 0)
        {
            _boxCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;

            _enemyAI.SetDeathState();
            OnDeath?.Invoke(this, EventArgs.Empty);
        }

    }



}
