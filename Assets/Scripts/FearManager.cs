
using UnityEngine;
using System.Collections.Generic;

public class FearManager : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private int currentFearLevel = 0;
    private List<IFearObserver> observers = new List<IFearObserver>();

    public int CurrentFearLevel
    {
        get => currentFearLevel;
        set
        {
            currentFearLevel = Mathf.Clamp(value, 0, 100);
            NotifyObservers();
        }
    }

    public void RegisterObserver(IFearObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void UnregisterObserver(IFearObserver observer)
    {
        if (observers.Contains(observer))
            observers.Remove(observer);
    }

    private void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.OnFearLevelChanged(currentFearLevel);
        }
    }
    
    /// <summary>
    /// Notifica el nivel de miedo inicial a todos los observers al arrancar la escena,
    /// sin necesitar un cambio de valor para disparar el primer spawn.
    /// </summary>
    private void Start()
    {
        NotifyObservers();
    }

    // For manual slider testing or UI
    public void SetFearFromSlider(float value)
    {
        CurrentFearLevel = Mathf.RoundToInt(value);
    }
}
