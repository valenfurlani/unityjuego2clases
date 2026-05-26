using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth     => maxHealth;

    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        OnDamageTaken?.Invoke(amount);

        if (currentHealth <= 0)
            Die();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth     = newMaxHealth;
        currentHealth = newMaxHealth;
    }

    [ContextMenu("[TEST] Forzar muerte")]
    public void ForceKill()
    {
        if (currentHealth <= 0) return;
        currentHealth = 0;
        Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}
