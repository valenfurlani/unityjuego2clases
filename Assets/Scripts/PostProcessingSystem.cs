using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private FearConfigSO fearConfig;
    [SerializeField] private PostProcessingConfigSO ppConfig;

    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out chromaticAberration);
            globalVolume.profile.TryGet(out colorAdjustments);
        }
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
        if (fearConfig == null || ppConfig == null) return;

        int index = fearConfig.GetRangeIndex(fearLevel);
        if (index >= ppConfig.rangeConfigs.Length) return;

        PostProcessingRangeConfig config = ppConfig.rangeConfigs[index];

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = config.aberrationIntensity;

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = config.saturation;
            colorAdjustments.contrast.value = config.contrast;
        }
    }
}