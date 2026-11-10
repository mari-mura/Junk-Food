using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.PlayerLoop;

public class NightTimer : MonoBehaviour
{
    private static NightTimer instance;
    public static event Action OutOfTime;
    
    [SerializeField] private float startTime;
    private float remainingTime;
    private bool isCounting;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        StartTimer();
    }

    private void Update()
    {
        if (isCounting == false)
        {
            return;
        }
        
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0f;
            isCounting = false;

            OutOfTime.Invoke();
        }
    }

public static float RemainingTime
    {
        get
        {
            if (instance != null)
            {
                return instance.remainingTime;
            }

            return 0f;
        }
    }

    public static bool IsCounting
    {
        get
        {
            return instance.isCounting;
        }
    }

    public static void RestartTimer()
    {
        if (instance != null)
        {
            instance.StartTimer();
        }
    }

    private void StartTimer()
    {
        remainingTime = startTime;
        isCounting = true;
    }
    
    
}
