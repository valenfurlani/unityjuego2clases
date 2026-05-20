using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private Volume globalVolume;

    [SerializeField] private float maxAberration = 1f;
    [SerializeField] private float minSaturation = -80f;
    [SerializeField] private float maxContrast = 40f;
    
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
        float normalizedFear = Mathf.Clamp01(fearLevel / 100f);

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = normalizedFear * maxAberration;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = Mathf.Lerp(0f, minSaturation, normalizedFear);
            colorAdjustments.contrast.value = Mathf.Lerp(0f, maxContrast, normalizedFear);
        }
    }
}