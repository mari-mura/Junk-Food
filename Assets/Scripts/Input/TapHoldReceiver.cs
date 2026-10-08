using System;
using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class TapHoldReceiver : MonoBehaviour
{
    [SerializeField] TapHoldDetector detector;
    [SerializeField] bool logToConsole = false;
    
    public event Action<PressInfo> SingleTapped;
    
    public event Action<PressInfo> HoldStarted;
    
    public event Action<PressInfo> HoldEnded;
    
    public event Action<PressInfo> ChordTapped;

    void OnEnable()
    {
        detector.TapReleased += OnTapReleased;
        detector.HoldStarted += OnHoldStarted;
        detector.HoldEnded += OnHoldEnded;
    }

    void OnDisable()
    {
        detector.TapReleased -= OnTapReleased;
        detector.HoldStarted -= OnHoldStarted;
        detector.HoldEnded -= OnHoldEnded;
    }

    void OnTapReleased(PressInfo pressInfo)
    {
        if (pressInfo.IsChord)
        {
            Log("CHORD TAP " + string.Join("+", pressInfo.Buttons));
            if (ChordTapped != null)
            {
                ChordTapped.Invoke(pressInfo);
            }
        }
        else
        {
            Log("TAP " + pressInfo.Buttons[0]);
            if (SingleTapped != null)
            {
                SingleTapped.Invoke(pressInfo);
            }
        }
    }

    void OnHoldStarted(PressInfo pressInfo)
    {
        if (pressInfo.IsChord)
        {
            return;
        }

        Log("HOLD START " + pressInfo.Buttons[0]);
        if (HoldStarted != null)
        {
            HoldStarted.Invoke(pressInfo);
        }
    }

    void OnHoldEnded(PressInfo pressInfo)
    {
        if (pressInfo.IsChord)
        {
            return;
        }

        Log("HOLD END " + pressInfo.Buttons[0]);
        if (HoldEnded != null)
        {
            HoldEnded.Invoke(pressInfo);
        }
    }

    void Log(string message)
    {
        if (logToConsole)
        {
            Debug.Log("[Rhythm] " + message);
        }
    }
}