using UnityEngine;
using UnityEngine.UI;

public class BurdenMeter : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private Slider slider;
    // burden pts per sec
    [SerializeField] private float fillSpeed = 30f;

    private void OnEnable()
    {
        slider.minValue = 0f;
        slider.maxValue = inventoryData.MaxBurden;
        slider.value = inventoryData.GetBurden();
    }

    private void Update()
    {
        float target = inventoryData.GetBurden();
        slider.value = Mathf.MoveTowards(slider.value, target, fillSpeed * Time.unscaledDeltaTime);
    }
}