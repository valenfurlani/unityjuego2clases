using UnityEngine;
using UnityEngine.UI;

public class FearSliderConnector : MonoBehaviour
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null || fearManager == null) return;

        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.wholeNumbers = true;
        slider.value = fearManager.CurrentFearLevel;

        slider.onValueChanged.AddListener(fearManager.SetFearFromSlider);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(fearManager.SetFearFromSlider);
    }
}
