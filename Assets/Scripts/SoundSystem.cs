using UnityEngine;
using System.Collections;

public class SoundSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private FearConfigSO fearConfig;
    [SerializeField] private AudioConfigSO audioConfig;
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
        if (fearConfig == null || audioConfig == null || trackSources == null) return;

        int newRangeIndex = fearConfig.GetRangeIndex(fearLevel);
        if (newRangeIndex == currentRangeIndex) return;

        if (blendCoroutine != null) StopCoroutine(blendCoroutine);
        blendCoroutine = StartCoroutine(CrossfadeTracks(currentRangeIndex, newRangeIndex));
        currentRangeIndex = newRangeIndex;
    }

    private void InitializeTracks()
    {
        if (audioConfig == null || trackSources == null) return;

        for (int i = 0; i < audioConfig.rangeConfigs.Length && i < trackSources.Length; i++)
        {
            trackSources[i].volume = 0f;

            if (audioConfig.rangeConfigs[i].musicClip != null)
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

        float fromStartVolume = (fromIndex >= 0 && fromIndex < trackSources.Length)
            ? trackSources[fromIndex].volume
            : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (fromIndex >= 0 && fromIndex < trackSources.Length)
                trackSources[fromIndex].volume = Mathf.Lerp(fromStartVolume, 0f, t);

            if (toIndex >= 0 && toIndex < trackSources.Length)
                trackSources[toIndex].volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        if (fromIndex >= 0 && fromIndex < trackSources.Length)
            trackSources[fromIndex].volume = 0f;

        if (toIndex >= 0 && toIndex < trackSources.Length)
            trackSources[toIndex].volume = 1f;
    }
}
