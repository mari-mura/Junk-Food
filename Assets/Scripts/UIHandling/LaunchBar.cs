using UnityEngine;
using UnityEngine.UI;

public class LaunchBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    
    public void Show(){
        gameObject.SetActive(true);
        slider.value = 0f;
    }

    public void SetFill(float percent)
    {
        slider.value = percent;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
