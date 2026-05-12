
using UnityEngine;

public class SoundSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    
    private void OnEnable() => fearManager.RegisterObserver(this);
    private void OnDisable() => fearManager.UnregisterObserver(this);

    public void OnFearLevelChanged(int fearLevel)
    {
        // logic for volume or music changes
        Debug.Log($"SoundSystem: Adjusting audio for fear level {fearLevel}");
    }
}
