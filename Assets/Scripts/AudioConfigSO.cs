using UnityEngine;

[System.Serializable]
public class AudioRangeConfig
{
    public AudioClip musicClip;
    public float transitionDuration;
}

[CreateAssetMenu(fileName = "NewAudioConfig", menuName = "FearSystem/AudioConfig")]
public class AudioConfigSO : ScriptableObject
{
    public AudioRangeConfig[] rangeConfigs;
}
