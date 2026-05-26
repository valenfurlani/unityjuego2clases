using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour, ITimerObserver
{
    [Header("Paneles a mostrar al morir (dejarlos INACTIVOS en el editor)")]
    [SerializeField] private GameObject textsPanel;
    [SerializeField] private GameObject buttonPanel;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI sessionTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private GameObject      newRecordLabel;

    [Header("Timer")]
    [SerializeField] private GameTimer gameTimer;

    [Header("Escena")]
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

        if (textsPanel  != null) textsPanel.SetActive(false);
        if (buttonPanel != null) buttonPanel.SetActive(false);
        if (newRecordLabel != null) newRecordLabel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gameTimer != null) gameTimer.UnregisterObserver(this);
    }

    public void OnTimerUpdated(float elapsedSeconds) { }

    public void OnTimerStopped(float finalSeconds)
    {
        bool isNewRecord = RecordService.SubmitTime(finalSeconds);
        ShowPanel(finalSeconds, isNewRecord);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    [ContextMenu("[TEST] Forzar Game Over")]
    public void ForceShowGameOver()
    {
        float t = gameTimer != null ? gameTimer.ElapsedTime : 99f;
        bool isNewRecord = RecordService.SubmitTime(t);
        ShowPanel(t, isNewRecord);
    }

    private void ShowPanel(float sessionSeconds, bool isNewRecord)
    {
        Time.timeScale = 0f;

        if (fearBarHUD != null) fearBarHUD.SetActive(false);
        if (timerHUD   != null) timerHUD.SetActive(false);

        if (textsPanel  != null) textsPanel.SetActive(true);
        if (buttonPanel != null) buttonPanel.SetActive(true);

        if (sessionTimeText != null)
            sessionTimeText.text = RecordService.FormatTime(sessionSeconds);

        if (bestTimeText != null)
            bestTimeText.text = RecordService.FormatTime(RecordService.BestTime);

        if (newRecordLabel != null)
            newRecordLabel.SetActive(isNewRecord);
    }
}
