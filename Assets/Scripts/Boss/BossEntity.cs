using System;
using UnityEngine;

public class BossEntity : MonoBehaviour
{
    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;
    public event EventHandler OnHealthChanged;

    [SerializeField] private int _maxHealth = 50;
    private int _currentHealth;

    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private BossAI _bossAI;

    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;
    public float GetHealthPercent() => (float)_currentHealth / _maxHealth;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _bossAI = GetComponent<BossAI>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        if (_currentHealth < 0) _currentHealth = 0;

        Debug.Log($"[Boss] HP: {_currentHealth}/{_maxHealth} ({GetHealthPercent() * 100:F0}%)");

        OnTakeHit?.Invoke(this, EventArgs.Empty);
        OnHealthChanged?.Invoke(this, EventArgs.Empty);

        DetectDeath();
    }

    public void PolygonColliderTurnOff()
    {
        if (_polygonCollider2D != null)
            _polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        if (_polygonCollider2D != null)
            _polygonCollider2D.enabled = true;
    }

    private void DetectDeath()
    {
        if (_currentHealth > 0) return;

        if (_boxCollider2D != null)
            _boxCollider2D.enabled = false;

        if (_polygonCollider2D != null)
            _polygonCollider2D.enabled = false;

        if (_bossAI != null)
            _bossAI.SetDeathState();

        OnDeath?.Invoke(this, EventArgs.Empty);

        Debug.Log("[Boss] ☠ БОСС ПОБЕЖДЁН!");
    }
}
