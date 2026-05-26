using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsabilidad única: mantener sincronizado el Slider de UI con el FearManager.
///
/// Flujo bidireccional:
///   - FearManager → Slider : implementa IFearObserver para reflejar cambios de código.
///   - Slider → FearManager : onValueChanged llama a SetFearFromSlider (para testing manual).
/// </summary>
public class FearSliderConnector : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private Slider slider;

    private bool _isSyncing = false; // evita bucle infinito slider ↔ manager

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null || fearManager == null) return;

        slider.minValue    = 0f;
        slider.maxValue    = 100f;
        slider.wholeNumbers = true;
        slider.value       = fearManager.CurrentFearLevel;

        slider.onValueChanged.AddListener(OnSliderChanged);
        fearManager.RegisterObserver(this);
    }

    private void OnDestroy()
    {
        if (slider     != null) slider.onValueChanged.RemoveListener(OnSliderChanged);
        if (fearManager != null) fearManager.UnregisterObserver(this);
    }

    // ── IFearObserver ─────────────────────────────────────────────────────────

    /// <summary>El FearManager cambió (por código/daño): actualiza el slider sin disparar el listener.</summary>
    public void OnFearLevelChanged(int fearLevel)
    {
        if (slider == null) return;

        _isSyncing  = true;
        slider.value = fearLevel;
        _isSyncing  = false;
    }

    // ── Slider → FearManager (testing manual) ─────────────────────────────────

    private void OnSliderChanged(float value)
    {
        if (_isSyncing || fearManager == null) return;
        fearManager.SetFearFromSlider(value);
    }
}
