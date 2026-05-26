using UnityEngine;
using Unity.Cinemachine;

public class CameraSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private FearConfigSO fearConfig;
    [SerializeField] private CameraConfigSO cameraConfig;

    private float baseOrthoSize;

    private void Awake()
    {
        if (virtualCamera != null)
            baseOrthoSize = virtualCamera.Lens.OrthographicSize;
    }

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
        if (fearConfig == null || cameraConfig == null) return;

        int index = fearConfig.GetRangeIndex(fearLevel);
        if (index >= cameraConfig.rangeConfigs.Length) return;

        CameraRangeConfig config = cameraConfig.rangeConfigs[index];

        if (config.shakeForce > 0 && impulseSource != null)
            impulseSource.GenerateImpulseWithForce(config.shakeForce);

        if (virtualCamera != null)
        {
            var lens = virtualCamera.Lens;
            lens.OrthographicSize = baseOrthoSize + config.zoomOffset;
            virtualCamera.Lens = lens;
        }
    }
}