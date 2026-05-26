using UnityEngine;

[System.Serializable]
public class CameraRangeConfig
{
    public float shakeForce;

    [Header("Respiración de zoom")]
    [Tooltip("Cuánto se aleja/acerca la cámara en cada ciclo (0 = sin efecto).")]
    public float breatheAmplitude = 0f;
    [Tooltip("Velocidad de la respiración en ciclos por segundo.")]
    public float breatheSpeed     = 1f;
}

[CreateAssetMenu(fileName = "NewCameraConfig", menuName = "FearSystem/CameraConfig")]
public class CameraConfigSO : ScriptableObject
{
    public CameraRangeConfig[] rangeConfigs;
}
