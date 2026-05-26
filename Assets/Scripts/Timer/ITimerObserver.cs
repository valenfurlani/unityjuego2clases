/// <summary>
/// Observer que reacciona a los cambios del GameTimer.
/// Principio de Segregación de Interfaces (ISP): interfaz pequeña y enfocada.
/// </summary>
public interface ITimerObserver
{
    /// <summary>Se llama cada vez que el tiempo transcurrido cambia.</summary>
    void OnTimerUpdated(float elapsedSeconds);

    /// <summary>Se llama cuando el timer se detiene (muerte del jugador).</summary>
    void OnTimerStopped(float finalSeconds);
}
