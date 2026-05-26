using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Pantalla de Game Over. 
/// IMPORTANTE: este script debe estar en un GameObject SIEMPRE ACTIVO
/// (no en el panel que se oculta). Los paneles hijos se controlan por referencia.
/// Deja los paneles inactivos directamente en el editor — este script los activa al morir.
/// </summary>
public class GameOverUI : MonoBehaviour, ITimerObserver
{
    [Header("Paneles a mostrar al morir (dejarlos INACTIVOS en el editor)")]
    [Tooltip("GameObject con los textos de tiempo.")]
    [SerializeField] private GameObject textsPanel;
    [Tooltip("GameObject con el botón de volver al menú.")]
    [SerializeField] private GameObject buttonPanel;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI sessionTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private GameObject      newRecordLabel;

    [Header("Timer")]
    [SerializeField] private GameTimer gameTimer;

    [Header("Escena")]
    [Tooltip("Nombre exacto de la escena del menú principal.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("HUD a ocultar al morir")]
    [SerializeField] private GameObject fearBarHUD;
    [SerializeField] private GameObject timerHUD;

    private void Start()
    {
        if (gameTimer != null)
        {
            gameTimer.RegisterObserver(this);
        }

        // Asegura que los paneles empiezan ocultos
        if (textsPanel  != null) textsPanel.SetActive(false);
        if (buttonPanel != null) buttonPanel.SetActive(false);
        if (newRecordLabel != null) newRecordLabel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gameTimer != null) gameTimer.UnregisterObserver(this);
    }

    // ── ITimerObserver ────────────────────────────────────────────────────────

    public void OnTimerUpdated(float elapsedSeconds) { }

    public void OnTimerStopped(float finalSeconds)
    {
        bool isNewRecord = RecordService.SubmitTime(finalSeconds);
        ShowPanel(finalSeconds, isNewRecord);
    }

    // ── Botones ───────────────────────────────────────────────────────────────

    /// <summary>Asigna al botón "Volver al Menú".</summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;   // restaura antes de cambiar de escena
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>Testea la pantalla desde el Inspector (clic derecho → [TEST]).</summary>
    [ContextMenu("[TEST] Forzar Game Over")]
    public void ForceShowGameOver()
    {
        float t = gameTimer != null ? gameTimer.ElapsedTime : 99f;
        bool isNewRecord = RecordService.SubmitTime(t);
        ShowPanel(t, isNewRecord);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ShowPanel(float sessionSeconds, bool isNewRecord)
    {
        Time.timeScale = 0f;   // pausa el juego (física, animaciones, coroutines)

        // Oculta HUD de juego
        if (fearBarHUD != null) fearBarHUD.SetActive(false);
        if (timerHUD   != null) timerHUD.SetActive(false);

        // Muestra los dos paneles de Game Over
        if (textsPanel  != null) textsPanel.SetActive(true);
        if (buttonPanel != null) buttonPanel.SetActive(true);

        // Rellena textos
        if (sessionTimeText != null)
            sessionTimeText.text = RecordService.FormatTime(sessionSeconds);

        if (bestTimeText != null)
            bestTimeText.text = RecordService.FormatTime(RecordService.BestTime);

        if (newRecordLabel != null)
            newRecordLabel.SetActive(isNewRecord);
    }
}
