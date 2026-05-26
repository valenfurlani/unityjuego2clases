using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsabilidad única: mantener sincronizado el Slider de UI con la vida del jugador (Health).
/// </summary>
public class HealthSliderConnector : MonoBehaviour, IHealthObserver
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null || playerHealth == null) return;

        // Configuración inicial del Slider
        slider.minValue = 0f;
        slider.maxValue = playerHealth.MaxHealth;
        slider.value = playerHealth.CurrentHealth;

        // Registrar observador
        playerHealth.RegisterObserver(this);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.UnregisterObserver(this);
        }
    }

    // ── IHealthObserver ─────────────────────────────────────────────────────────

    public void OnHealthChanged(float currentHealth, float maxHealth)
    {
        if (slider == null) return;

        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }
}
