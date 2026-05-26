using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class SoundSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private FearConfigSO fearConfig;
    [SerializeField] private AudioConfigSO audioConfig;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource[] trackSources;

    private int currentRangeIndex = -1;
    private Coroutine blendCoroutine;

    private void Awake()
    {
        InitializeTracks();
    }

    private void OnEnable() => fearManager.RegisterObserver(this);
    private void OnDisable() => fearManager.UnregisterObserver(this);

    public void OnFearLevelChanged(int fearLevel)
    {
        if (fearConfig == null || audioConfig == null) return;

        int newRangeIndex = fearConfig.GetRangeIndex(fearLevel);
        if (newRangeIndex == currentRangeIndex) return;

        if (blendCoroutine != null) StopCoroutine(blendCoroutine);
        blendCoroutine = StartCoroutine(CrossfadeTracks(currentRangeIndex, newRangeIndex));
        currentRangeIndex = newRangeIndex;
    }

    private void InitializeTracks()
    {
        if (audioConfig == null || audioMixer == null) return;

        for (int i = 0; i < audioConfig.rangeConfigs.Length; i++)
        {
            SetTrackVolume(audioConfig.rangeConfigs[i].mixerVolumeParam, 0f);

            if (i < trackSources.Length && audioConfig.rangeConfigs[i].musicClip != null)
            {
                trackSources[i].clip = audioConfig.rangeConfigs[i].musicClip;
                trackSources[i].loop = true;
                trackSources[i].Play();
            }
        }
    }

    private IEnumerator CrossfadeTracks(int fromIndex, int toIndex)
    {
        float duration = audioConfig.rangeConfigs[toIndex].transitionDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (fromIndex >= 0 && fromIndex < audioConfig.rangeConfigs.Length)
                SetTrackVolume(audioConfig.rangeConfigs[fromIndex].mixerVolumeParam, 1f - t);

            SetTrackVolume(audioConfig.rangeConfigs[toIndex].mixerVolumeParam, t);
            yield return null;
        }

        if (fromIndex >= 0 && fromIndex < audioConfig.rangeConfigs.Length)
            SetTrackVolume(audioConfig.rangeConfigs[fromIndex].mixerVolumeParam, 0f);

        SetTrackVolume(audioConfig.rangeConfigs[toIndex].mixerVolumeParam, 1f);
    }

    private void SetTrackVolume(string paramName, float linearVolume)
    {
        float dB = linearVolume > 0.0001f ? Mathf.Log10(linearVolume) * 20f : -80f;
        audioMixer.SetFloat(paramName, dB);
    }
}
