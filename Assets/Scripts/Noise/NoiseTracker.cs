using System;
using UnityEngine;

public static class NoiseTracker
{
    public const float MaxNoise = 100f;
    
    public static event Action<float> NoiseUpdated;

    private static float noise;

    public static float Noise
    {
        get { return noise; }
    }

    public static void Increase(float amount)
    {
        SetNoise(noise + amount);
    }

    public static void Decrease(float amount)
    {
        SetNoise(noise - amount);
    }

    public static void Reset()
    {
        SetNoise(0f);
    }
    private static void SetNoise(float value)
    {
        float oldNoise = noise;
        noise = Mathf.Clamp(value, 0f, MaxNoise);
        NoiseUpdated?.Invoke(noise);

        if (oldNoise < MaxNoise && noise >= MaxNoise)
        {
            Debug.Log("You got caught!");
        }
    }
    
}
