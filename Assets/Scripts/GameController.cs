using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    int burdenAmount;
    public Slider burdenSlider;
    public AudioClip pickupSound;
    public Transform bag;
    
    public float volume = 0.2f;
    public float volumeIncrease = 0.05f;
    public float growAmount = 0.1f;

    void OnEnable()
    {
        Trash.OnTrashCollect += IncreaseBurdenAmount;
    }

    void OnDisable()
    {
        Trash.OnTrashCollect -= IncreaseBurdenAmount;
    }
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
        bag.localScale += new Vector3(growAmount, growAmount, 0);
        volume += volumeIncrease;
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        
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
