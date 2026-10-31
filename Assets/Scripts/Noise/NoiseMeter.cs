using UnityEngine;
using UnityEngine.UI;

public class NoiseMeter : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        NoiseTracker.NoiseUpdated += OnNoiseUpdated;
        slider.value = NoiseTracker.Noise;
    }

    private void OnDisable()
    {
        NoiseTracker.NoiseUpdated -= OnNoiseUpdated;
    }

    private void OnNoiseUpdated(float noise)
    {
        slider.value = noise;
    }

    public void ResetMeter()
    {
        slider.value = 0f;
    }
}
