using UnityEngine;
using Unity.Cinemachine;

public class CameraSystem : MonoBehaviour, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private FearConfigSO fearConfig;
    [SerializeField] private CameraConfigSO cameraConfig;

    [SerializeField] private float transitionSpeed = 2f;

    private Transform _playerTransform;
    private GameObject _swayTarget;
    private Vector2 _currentSwayOffset;

    private float _currentAmplitude;
    private float _currentSpeed;

    private float _targetAmplitude;
    private float _targetSpeed;

    private float _phase;

    private void Start()
    {
        if (virtualCamera != null && virtualCamera.Follow != null)
        {
            _playerTransform = virtualCamera.Follow;
            
            _swayTarget = new GameObject("[CameraSwayTarget]");
            _swayTarget.transform.position = _playerTransform.position;
            
            virtualCamera.Follow = _swayTarget.transform;
        }
    }

    private void OnDestroy()
    {
        if (_swayTarget != null)
        {
            Destroy(_swayTarget);
        }
    }

    private void OnEnable()  => fearManager?.RegisterObserver(this);
    private void OnDisable() => fearManager?.UnregisterObserver(this);

    public void OnFearLevelChanged(int fearLevel)
    {
        if (fearConfig == null || cameraConfig == null) return;

        int index = fearConfig.GetRangeIndex(fearLevel);
        if (index >= cameraConfig.rangeConfigs.Length) return;

        CameraRangeConfig config = cameraConfig.rangeConfigs[index];

        if (config.shakeForce > 0 && impulseSource != null)
            impulseSource.GenerateImpulseWithForce(config.shakeForce);

        _targetAmplitude = config.breatheAmplitude;
        _targetSpeed     = config.breatheSpeed;
    }

    private void Update()
    {
        if (_targetAmplitude == 0f)
        {
            _currentSwayOffset = Vector2.Lerp(_currentSwayOffset, Vector2.zero, Time.deltaTime * transitionSpeed);
            _currentAmplitude = 0f;
            _currentSpeed = 0f;
            _phase = 0f;
        }
        else
        {
            _currentAmplitude = Mathf.Lerp(_currentAmplitude, _targetAmplitude, Time.deltaTime * transitionSpeed);
            _currentSpeed     = Mathf.Lerp(_currentSpeed,     _targetSpeed,     Time.deltaTime * transitionSpeed);

            _phase += _currentSpeed * Time.deltaTime;

            float rawNoiseX = Mathf.PerlinNoise(_phase, 0f) - 0.5f;
            float rawNoiseY = Mathf.PerlinNoise(0f, _phase * 1.2f) - 0.5f;

            float swayX = rawNoiseX * 2f * _currentAmplitude;
            float swayY = rawNoiseY * 2f * _currentAmplitude;

            _currentSwayOffset = new Vector2(swayX, swayY);
        }
    }

    private void LateUpdate()
    {
        if (_playerTransform != null && _swayTarget != null)
        {
            _swayTarget.transform.position = _playerTransform.position + (Vector3)_currentSwayOffset;
        }
    }
}