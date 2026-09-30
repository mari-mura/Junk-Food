using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    int burdenAmount;
    public Slider burdenSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        burdenAmount = 0;
        burdenSlider.value = 0;
        Trash.OnTrashCollect += IncreaseBurdenAmount;
    }

    void IncreaseBurdenAmount(int amount)
    {
        burdenAmount += amount;
        burdenSlider.value = burdenAmount;
        if (burdenAmount >= 100)
        {
            Debug.Log("Level Complete");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
