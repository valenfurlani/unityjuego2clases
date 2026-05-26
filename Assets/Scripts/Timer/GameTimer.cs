using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsabilidad única: medir el tiempo transcurrido desde que inicia hasta que se detiene.
/// Principio Abierto/Cerrado: se extiende mediante observadores sin modificar esta clase.
/// Principio de Inversión de Dependencias: notifica a través de ITimerObserver, no a clases concretas.
/// </summary>
public class GameTimer : MonoBehaviour
{
    private float _elapsedTime;
    private bool _isRunning;

    private readonly List<ITimerObserver> _observers = new List<ITimerObserver>();

    // ── Propiedades públicas de solo lectura ──────────────────────────────────
    public float ElapsedTime => _elapsedTime;
    public bool IsRunning => _isRunning;

    // ── Control del timer ─────────────────────────────────────────────────────

    /// <summary>Inicia el conteo desde cero.</summary>
    public void StartTimer()
    {
        _elapsedTime = 0f;
        _isRunning = true;
    }

    /// <summary>Detiene el conteo y notifica a los observadores.</summary>
    public void StopTimer()
    {
        if (!_isRunning) return;

        _isRunning = false;
        NotifyTimerStopped();
    }

    // ── Registro de observadores ──────────────────────────────────────────────

    public void RegisterObserver(ITimerObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void UnregisterObserver(ITimerObserver observer)
    {
        _observers.Remove(observer);
    }

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Update()
    {
        if (!_isRunning) return;

        _elapsedTime += Time.deltaTime;
        NotifyTimerUpdated();
    }

    // ── Notificaciones privadas ───────────────────────────────────────────────

    private void NotifyTimerUpdated()
    {
        foreach (var observer in _observers)
            observer.OnTimerUpdated(_elapsedTime);
    }

    private void NotifyTimerStopped()
    {
        foreach (var observer in _observers)
            observer.OnTimerStopped(_elapsedTime);
    }
}
