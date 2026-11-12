using System;
using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class TapHoldReceiver : MonoBehaviour
{
    [SerializeField] TapHoldDetector detector;
    [SerializeField] ButtonHold buttonHold;
    [SerializeField] bool logToConsole = false;
    public PathSession pathSession;
    [SerializeField] private GameObject player;
    
    public event Action<PressInfo> SingleTapped;
    
    public event Action<PressInfo> HoldStarted;
    
    public event Action<PressInfo> HoldEnded;
    
    public event Action<PressInfo> ChordTapped;
    
    public TapHoldDetector Detector => detector;

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
            Debug.Log("CHORD TAP " + string.Join(",", pressInfo.Buttons));

            if (ChordTapped != null)
            {
                ChordTapped.Invoke(pressInfo);
            }
        }
        else
        {
            if (SingleTapped != null)
            {
                SingleTapped.Invoke(pressInfo);
            }
        }

        pathSession.SubmitTap(pressInfo.Buttons, player.transform.position);
    }

    void OnHoldStarted(PressInfo pressInfo)
    {
        Log("HOLD START " + string.Join(",", pressInfo.Buttons));
        buttonHold.StartHold();

        if (HoldStarted != null)
        {
            HoldStarted.Invoke(pressInfo);
        }
        
        pathSession.SubmitHoldStart(pressInfo.Buttons, player.transform.position);
    }

    void OnHoldEnded(PressInfo pressInfo)
    {
        Log("HOLD END " + string.Join(",", pressInfo.Buttons));
        buttonHold.StopHold();

        if (HoldEnded != null)
        {
            HoldEnded.Invoke(pressInfo);
        }
        
        pathSession.SubmitRelease(pressInfo.Buttons, player.transform.position);
    }

    void Log(string message)
    {
        if (logToConsole)
        {
           Debug.Log("[Rhythm] " + message);
        }
    }
    
}