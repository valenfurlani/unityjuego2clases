using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour, ITimerObserver
{
    [Header("Referencias")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Formato")]
    [SerializeField] private TimerDisplayFormat displayFormat = TimerDisplayFormat.MinutesSeconds;

    private void OnEnable()
    {
        if (gameTimer == null) return;
        gameTimer.RegisterObserver(this);
    }

    private void OnDisable()
    {
        if (gameTimer != null)
            gameTimer.UnregisterObserver(this);
    }

    public void OnTimerUpdated(float elapsedSeconds)
    {
        UpdateDisplay(elapsedSeconds);
    }

    public void OnTimerStopped(float finalSeconds)
    {
        UpdateDisplay(finalSeconds);
        if (timerText != null)
            timerText.color = new Color(1f, 0.3f, 0.3f);
    }

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
