using UnityEngine;

/// <summary>
/// Responsabilidad única: iniciar el timer cuando la escena esté lista (Awake/Start).
/// Permite iniciar el timer automáticamente al dar Play sin lógica extra en GameTimer.
/// </summary>
public class GameTimerStarter : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;

    private void Start()
    {
        if (gameTimer == null)
        {
            Debug.LogError($"[GameTimerStarter] Falta asignar el GameTimer en {gameObject.name}");
            return;
        }

        gameTimer.StartTimer();
    }
}
