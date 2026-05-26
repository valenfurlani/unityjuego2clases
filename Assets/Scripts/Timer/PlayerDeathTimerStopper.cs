using UnityEngine;

public class PlayerDeathTimerStopper : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameTimer gameTimer;

    private void OnEnable()
    {
        if (playerHealth == null || gameTimer == null) return;
        playerHealth.OnDeath.AddListener(HandlePlayerDeath);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDeath.RemoveListener(HandlePlayerDeath);
    }

    private void HandlePlayerDeath()
    {
        gameTimer.StopTimer();
    }
}
