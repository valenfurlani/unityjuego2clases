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

    private System.Collections.Generic.List<IHealthObserver> observers = new System.Collections.Generic.List<IHealthObserver>();

    public void RegisterObserver(IHealthObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void UnregisterObserver(IHealthObserver observer)
    {
        if (observers.Contains(observer))
            observers.Remove(observer);
    }

    private void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.OnHealthChanged(currentHealth, maxHealth);
        }
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        NotifyObservers();
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        OnDamageTaken?.Invoke(amount);
        NotifyObservers();

        if (currentHealth <= 0)
            Die();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        NotifyObservers();
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth     = newMaxHealth;
        currentHealth = newMaxHealth;
        NotifyObservers();
    }

    [ContextMenu("[TEST] Forzar muerte")]
    public void ForceKill()
    {
        if (currentHealth <= 0) return;
        currentHealth = 0;
        NotifyObservers();
        Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}
