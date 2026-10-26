using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField, Min(1)] private int maxHealth = 100;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int> Damaged;
    public event Action Died;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        Damaged?.Invoke(amount);

        if (IsDead) Die();
    }

    private void Die()
    {
        Died?.Invoke();
    }
}
