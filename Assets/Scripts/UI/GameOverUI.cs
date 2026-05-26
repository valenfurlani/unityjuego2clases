using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Responsabilidad única: mostrar la pantalla de Game Over con el tiempo
/// de la sesión, el mejor tiempo y si se rompió el record.
/// Se activa desde GameOverManager cuando el jugador muere.
/// </summary>
public class GameOverUI : MonoBehaviour, ITimerObserver
{
    [Header("Panel raíz (desactivado al inicio)")]
    [SerializeField] private GameObject panel;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI sessionTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private GameObject      newRecordLabel;   // objeto con texto "¡NUEVO RÉCORD!"

    [Header("Timer")]
    [SerializeField] private GameTimer gameTimer;

    [Header("Escena")]
    [Tooltip("Nombre exacto de la escena del menú principal.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("HUD a ocultar al morir")]
    [Tooltip("GameObject raíz de la barra de miedo (Slider o su panel).")]
    [SerializeField] private GameObject fearBarHUD;
    [Tooltip("GameObject raíz del timer en pantalla.")]
    [SerializeField] private GameObject timerHUD;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (newRecordLabel != null) newRecordLabel.SetActive(false);
    }

    private void OnEnable()
    {
        if (gameTimer != null) gameTimer.RegisterObserver(this);
    }

    private void OnDisable()
    {
        if (gameTimer != null) gameTimer.UnregisterObserver(this);
    }

    // ── ITimerObserver ────────────────────────────────────────────────────────

    public void OnTimerUpdated(float elapsedSeconds) { }   // no necesario aquí

    /// <summary>El timer se paró (jugador murió): muestra el panel con los datos.</summary>
    public void OnTimerStopped(float finalSeconds)
    {
        bool isNewRecord = RecordService.SubmitTime(finalSeconds);
        ShowPanel(finalSeconds, isNewRecord);
    }

    // ── Botones ───────────────────────────────────────────────────────────────

    /// <summary>Asigna al botón "Volver al Menú".</summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ShowPanel(float sessionSeconds, bool isNewRecord)
    {
        // Oculta el HUD de juego
        if (fearBarHUD != null) fearBarHUD.SetActive(false);
        if (timerHUD   != null) timerHUD.SetActive(false);

        if (panel != null) panel.SetActive(true);

        if (sessionTimeText != null)
            sessionTimeText.text = RecordService.FormatTime(sessionSeconds);

        if (bestTimeText != null)
            bestTimeText.text = RecordService.FormatTime(RecordService.BestTime);

        if (newRecordLabel != null)
            newRecordLabel.SetActive(isNewRecord);
    }
}
