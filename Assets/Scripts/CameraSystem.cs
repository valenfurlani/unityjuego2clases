using UnityEngine;
using Unity.Cinemachine;

public class CameraSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void OnEnable()
    {
        if (fearManager != null) fearManager.RegisterObserver(this);
    }

    private void OnDisable()
    {
        if (fearManager != null) fearManager.UnregisterObserver(this);
    }

    public void OnFearLevelChanged(int fearLevel)
    {
        if (fearLevel > 25)
        {
            TriggerShake(fearLevel);
        }
    }

    private void TriggerShake(int fearLevel)
    {
        float intensity = fearLevel / 100f;
        impulseSource.GenerateImpulseWithForce(intensity);
    }
}