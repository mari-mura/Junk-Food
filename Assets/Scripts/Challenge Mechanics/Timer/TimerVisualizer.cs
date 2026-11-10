using TMPro;
using UnityEngine;

public class TimerVisualizer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        int secondsLeft = Mathf.CeilToInt(NightTimer.RemainingTime);
        
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;
        timerText.text = minutes + ":" + seconds.ToString("00");
    }
}
