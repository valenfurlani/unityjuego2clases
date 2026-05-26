using UnityEngine;

public class GameTimerStarter : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;

    private void Start()
    {
        if (gameTimer == null) return;
        gameTimer.StartTimer();
    }
}
