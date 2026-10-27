using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] float remainingTime;
    void Update()
    {
        float backgroundChange = remainingTime;
        
        if (remainingTime > 0) {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0) {
            remainingTime = 0;
            countdownText.color = Color.red;
        }

        if (backgroundChange > 90 && remainingTime <= 90) Debug.Log("It got lighter");
        if (backgroundChange > 60 && remainingTime <= 60) Debug.Log("It got lighter");
        if (backgroundChange > 30 && remainingTime <= 30) Debug.Log("It got lighter");
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        countdownText.text = $"{minutes:00}:{seconds:00}";
    }
}
