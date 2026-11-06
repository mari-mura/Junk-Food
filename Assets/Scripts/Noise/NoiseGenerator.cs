using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] private float noiseMultiplier = 1f;

    [Header("Burden")]
    [SerializeField] private InventoryData inventoryData;
    [SerializeField, Min(0f)] private float burdenNoiseBoost = 1f;
    public void ApplyGrade(HitGrade grade)
    {
        float change = grade switch
        {
            HitGrade.Miss => 10f,
            HitGrade.Good => 4f,
            HitGrade.Great => 2f,
            HitGrade.Perfect => 1f
        };
        
        change *= noiseMultiplier;

        // the higher the burder the heavier the player
        if (inventoryData != null)
        {
            change *= 1f + inventoryData.GetBurden01() * burdenNoiseBoost;
        }
        
        if (change > 0f)
        {
            NoiseTracker.Increase(change);
        }
        else
        {
            NoiseTracker.Decrease(change);
        }
    }
    
}
