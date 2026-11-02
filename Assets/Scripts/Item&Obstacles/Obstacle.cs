using System;
using ScriptableObjects;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ObstacleData data;
    public static event Action<float> OnObstacleStep;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && data != null)
            OnObstacleStep?.Invoke(data.noiseAmount);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetEvent()
    {
        OnObstacleStep = null;
    }
}
