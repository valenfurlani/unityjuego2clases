public interface IHealthObserver
{
    void OnHealthChanged(float currentHealth, float maxHealth);
}
