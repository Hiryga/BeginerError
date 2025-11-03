// PlayerHealth.cs
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"[Player] HP: {currentHealth}/{maxHealth}");

        OnTakeHit?.Invoke(this, EventArgs.Empty);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true; 
            Die();  
        }
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        // Можно добавить: отключить движение, анимацию смерти
        Player.Instance.enabled = false;
        // Или: GameOver UI
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}