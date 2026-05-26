using UnityEngine;

[System.Serializable]
public class CameraRangeConfig
{
    public float shakeForce;
    public float shakeInterval;
    public float zoomOffset;
}

[CreateAssetMenu(fileName = "NewCameraConfig", menuName = "FearSystem/CameraConfig")]
public class CameraConfigSO : ScriptableObject
{
    public CameraRangeConfig[] rangeConfigs;
}
