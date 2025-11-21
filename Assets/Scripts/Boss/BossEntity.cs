using System;
using UnityEngine;

public class BossEntity : MonoBehaviour
{
    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    [SerializeField] private int _maxHealth = 50;
    private int _currentHealth;

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;

    public void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;
        if (_currentHealth < 0) _currentHealth = 0;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        // Здесь можно добавить анимацию или очистку объекта
    }
}
