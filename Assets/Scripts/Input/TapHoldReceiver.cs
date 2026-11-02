using System;
using UnityEngine;
using UnityEngine.InputSystem;
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
        Debug.Log("In On Tap released");
        if (pressInfo.IsChord)
        {
            Debug.Log("IsChord");

            //if (ChordTapped != null)
            //{
                Debug.Log("ChordTapped");
                pathSession.SubmitPress(pressInfo.Buttons, player.transform.position);
                //ChordTapped.Invoke(pressInfo);
            //}
        }
        else
        {
            Debug.Log("Not Chord");
            //if (SingleTapped != null)
            //{
                Debug.Log("SingleTapped");
                pathSession.SubmitPress(pressInfo.Buttons, player.transform.position);
                //SingleTapped.Invoke(pressInfo);
            //}
        }
    }

    void OnHoldStarted(PressInfo pressInfo)
    {
        Log("HOLD START " + pressInfo.Buttons[0]);
        buttonHold.StartHold();

        if (HoldStarted != null)
        {
            HoldStarted.Invoke(pressInfo);
        }
        
        pathSession.SubmitPress(pressInfo.Buttons, player.transform.position);
    }

    void OnHoldEnded(PressInfo pressInfo)
    {
        Log("HOLD END " + pressInfo.Buttons[0]);
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