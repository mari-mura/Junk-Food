using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NoiseTracker
{
    public const float MaxNoise = 100f;
    
    public static event Action<float> NoiseUpdated;
    public static event Action Caught;

    private static float noise;

    static NoiseTracker()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Reset();
        Debug.Log("Noise reset");
    }
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
            Caught?.Invoke();
            Debug.Log("You got caught!");
        }
    }
    
}
