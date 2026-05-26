using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    /// <summary>Vida actual. Útil para health bars y debugging en Play Mode.</summary>
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

    /// <summary>
    /// Cambia la vida máxima y reinicia la vida actual al nuevo valor.
    /// Úsalo al inicializar un enemigo desde su ScriptableObject.
    /// El jugador NO debe llamar a este método — usa el valor del Inspector.
    /// </summary>
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth     = newMaxHealth;
        currentHealth = newMaxHealth;
    }


    /// <summary>Mata al objeto instantáneamente. Útil para testing desde el Inspector (clic derecho en el componente).</summary>
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
