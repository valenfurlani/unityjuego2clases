using UnityEngine;

[System.Serializable]
public class PostProcessingRangeConfig
{
    public float aberrationIntensity;
    public float saturation;
    public float contrast;
}

[CreateAssetMenu(fileName = "NewPostProcessingConfig", menuName = "FearSystem/PostProcessingConfig")]
public class PostProcessingConfigSO : ScriptableObject
{
    public PostProcessingRangeConfig[] rangeConfigs;
}
