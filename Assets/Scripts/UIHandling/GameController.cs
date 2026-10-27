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

    public Slider noiseMeter;
    public float noisePerPickup = 10f;
    public float noiseDecay = 2f;
    private float noiseLevel;
    
    void OnEnable()
    {
        Trash.OnTrashCollect += IncreaseBurdenAmount;
        Obstacle.OnObstacleStep += AddNoise;
    }

    void OnDisable()
    {
        Trash.OnTrashCollect -= IncreaseBurdenAmount;
        Obstacle.OnObstacleStep -= AddNoise;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        burdenAmount = 0;
        burdenSlider.value = 0;
        burdenSlider.interactable = false;
        
        noiseLevel = 0;
        noiseMeter.value = 0;
        noiseMeter.interactable = false;
    }

    void IncreaseBurdenAmount(int amount)
    {
        burdenAmount += amount;
        burdenSlider.value = burdenAmount;
        bag.localScale += new Vector3(growAmount, growAmount, 0);
        volume += volumeIncrease;
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);

        AddNoise(noisePerPickup);
        
        if (burdenAmount >= 100)
        {
            Debug.Log("Level Complete");
        }
    }

    void AddNoise(float amount)
    {
        noiseLevel = Mathf.Clamp(noiseLevel + amount, 0, 100);
        noiseMeter.value = noiseLevel;

        if (noiseLevel >= 100)
        {
            Debug.Log("too loud!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (noiseLevel > 0 && noiseDecay > 0)
        {
            noiseLevel = Mathf.Max(noiseLevel - noiseDecay * Time.deltaTime, 0);
            noiseMeter.value = noiseLevel;
        }
    }
}
