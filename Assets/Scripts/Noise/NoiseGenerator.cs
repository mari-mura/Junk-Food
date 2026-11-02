using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] private float noiseMultiplier = 1f;

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
