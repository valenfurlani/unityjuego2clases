using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Responsabilidad única: controlar el comportamiento de la cámara según el nivel de miedo.
/// En lugar de cambiar el zoom, aplica un balanceo (sway) de cámara en forma de curva de Lissajous (infinito)
/// que simula el pulso inestable al apuntar bajo estrés o miedo.
/// </summary>
public class CameraSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private FearConfigSO fearConfig;
    [SerializeField] private CameraConfigSO cameraConfig;

    [Tooltip("Segundos que tardan amplitud y velocidad en hacer transición al nuevo rango.")]
    [SerializeField] private float transitionSpeed = 2f;

    // Referencias para el balanceo (sway)
    private Transform _playerTransform;
    private GameObject _swayTarget;
    private Vector2 _currentSwayOffset;

    // Valores actuales (se interpolan suavemente al cambiar de rango)
    private float _currentAmplitude;
    private float _currentSpeed;

    // Valores objetivo del rango activo
    private float _targetAmplitude;
    private float _targetSpeed;

    // Acumulador de fase para la onda continua
    private float _phase;

    private void Start()
    {
        if (virtualCamera != null && virtualCamera.Follow != null)
        {
            // Guardamos el player original y creamos un seguidor intermedio con balanceo
            _playerTransform = virtualCamera.Follow;
            
            _swayTarget = new GameObject("[CameraSwayTarget]");
            _swayTarget.transform.position = _playerTransform.position;
            
            // Hacemos que la cámara siga al target intermedio
            virtualCamera.Follow = _swayTarget.transform;
        }
    }

    private void OnDestroy()
    {
        // Limpieza del target creado en runtime
        if (_swayTarget != null)
        {
            Destroy(_swayTarget);
        }
    }

    private void OnEnable()  => fearManager?.RegisterObserver(this);
    private void OnDisable() => fearManager?.UnregisterObserver(this);

    // ── IFearObserver ─────────────────────────────────────────────────────────

    public void OnFearLevelChanged(int fearLevel)
    {
        if (fearConfig == null || cameraConfig == null) return;

        int index = fearConfig.GetRangeIndex(fearLevel);
        if (index >= cameraConfig.rangeConfigs.Length) return;

        CameraRangeConfig config = cameraConfig.rangeConfigs[index];

        // Shake puntual al cambiar de rango
        if (config.shakeForce > 0 && impulseSource != null)
            impulseSource.GenerateImpulseWithForce(config.shakeForce);

        // Nuevos objetivos de balanceo (mapeados de los campos de respiración)
        _targetAmplitude = config.breatheAmplitude;
        _targetSpeed     = config.breatheSpeed;
    }

    // ── Update & LateUpdate: cálculo del balanceo y posicionamiento ──────────

    private void Update()
    {
        if (_targetAmplitude == 0f)
        {
            // Retorno suave al centro
            _currentSwayOffset = Vector2.Lerp(_currentSwayOffset, Vector2.zero, Time.deltaTime * transitionSpeed);
            _currentAmplitude = 0f;
            _currentSpeed = 0f;
            _phase = 0f;
        }
        else
        {
            // Interpola suavidad del balanceo
            _currentAmplitude = Mathf.Lerp(_currentAmplitude, _targetAmplitude, Time.deltaTime * transitionSpeed);
            _currentSpeed     = Mathf.Lerp(_currentSpeed,     _targetSpeed,     Time.deltaTime * transitionSpeed);

            _phase += _currentSpeed * Time.deltaTime;

            // Usamos ruido Perlin desfasado para obtener un movimiento orgánico e impredecible
            // en todas las direcciones (arriba, abajo, izquierda, derecha y diagonales).
            float rawNoiseX = Mathf.PerlinNoise(_phase, 0f) - 0.5f;
            float rawNoiseY = Mathf.PerlinNoise(0f, _phase * 1.2f) - 0.5f;

            // Escalamos el ruido (de -0.5..0.5 lo llevamos a -1..1 y luego por la amplitud)
            float swayX = rawNoiseX * 2f * _currentAmplitude;
            float swayY = rawNoiseY * 2f * _currentAmplitude;

            _currentSwayOffset = new Vector2(swayX, swayY);
        }
    }

    private void LateUpdate()
    {
        if (_playerTransform != null && _swayTarget != null)
        {
            // El target intermedio sigue al player más el balanceo calculado
            _swayTarget.transform.position = _playerTransform.position + (Vector3)_currentSwayOffset;
        }
    }
}