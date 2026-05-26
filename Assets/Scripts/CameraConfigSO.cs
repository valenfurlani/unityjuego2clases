using UnityEngine;

[System.Serializable]
public class CameraRangeConfig
{
    public float shakeForce;

    [Header("Respiración de zoom")]
    public float breatheAmplitude = 0f;
    public float breatheSpeed     = 1f;
}

[CreateAssetMenu(fileName = "NewCameraConfig", menuName = "FearSystem/CameraConfig")]
public class CameraConfigSO : ScriptableObject
{
    public CameraRangeConfig[] rangeConfigs;
}
