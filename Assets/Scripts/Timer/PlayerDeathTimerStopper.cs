using UnityEngine;

/// <summary>
/// Responsabilidad única: escuchar la muerte del jugador y parar el timer.
/// Conecta el evento OnDeath del componente Health con el GameTimer.
/// Principio de Inversión de Dependencias: habla con GameTimer, no con la UI.
/// </summary>
public class PlayerDeathTimerStopper : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameTimer gameTimer;

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            Debug.LogError($"[PlayerDeathTimerStopper] Falta asignar el componente Health en {gameObject.name}");
            return;
        }

        if (gameTimer == null)
        {
            Debug.LogError($"[PlayerDeathTimerStopper] Falta asignar el GameTimer en {gameObject.name}");
            return;
        }

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
