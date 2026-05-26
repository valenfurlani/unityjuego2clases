using TMPro;
using UnityEngine;

/// <summary>
/// Responsabilidad única: mostrar el tiempo en pantalla.
/// Implementa ITimerObserver para recibir actualizaciones sin acoplarse a GameTimer directamente.
/// Principio Abierto/Cerrado: GameTimer no necesita cambiar para agregar esta UI.
/// </summary>
public class TimerUI : MonoBehaviour, ITimerObserver
{
    [Header("Referencias")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Formato")]
    [Tooltip("Formato de tiempo: 'mm:ss' o 'ss.ff'")]
    [SerializeField] private TimerDisplayFormat displayFormat = TimerDisplayFormat.MinutesSeconds;

    private void OnEnable()
    {
        if (gameTimer == null)
        {
            Debug.LogError($"[TimerUI] Falta asignar el GameTimer en {gameObject.name}");
            return;
        }

        gameTimer.RegisterObserver(this);
    }

    private void OnDisable()
    {
        if (gameTimer != null)
            gameTimer.UnregisterObserver(this);
    }

    // ── ITimerObserver ────────────────────────────────────────────────────────

    public void OnTimerUpdated(float elapsedSeconds)
    {
        UpdateDisplay(elapsedSeconds);
    }

    public void OnTimerStopped(float finalSeconds)
    {
        UpdateDisplay(finalSeconds);
        // Opcional: cambiar color para indicar que el timer está detenido
        if (timerText != null)
            timerText.color = new Color(1f, 0.3f, 0.3f); // rojo al morir
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void UpdateDisplay(float seconds)
    {
        if (timerText == null) return;
        timerText.text = FormatTime(seconds);
    }

    private string FormatTime(float seconds)
    {
        return displayFormat switch
        {
            TimerDisplayFormat.MinutesSeconds => $"{Mathf.FloorToInt(seconds / 60):00}:{Mathf.FloorToInt(seconds % 60):00}",
            TimerDisplayFormat.SecondsMilliseconds => $"{Mathf.FloorToInt(seconds):00}.{Mathf.FloorToInt((seconds % 1) * 100):00}",
            _ => $"{seconds:F1}s"
        };
    }
}

public enum TimerDisplayFormat
{
    MinutesSeconds,
    SecondsMilliseconds,
    RawSeconds
}
