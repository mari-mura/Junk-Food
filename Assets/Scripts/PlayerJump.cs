using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using PressInfo = TapHoldDetector.PressInfo;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] TapHoldReceiver receiver;
    [SerializeField] Vector3 jumpOffset = new Vector3(2f, 0f, 0f);
    [SerializeField] float jumpPower = 2f;
    [SerializeField] float jumpDuration = 0.8f;

    Sequence jumpSequence;

    void OnEnable()
    {
        receiver.SingleTapped += HandleSingleTapped;
        receiver.HoldStarted += HandleHoldStarted;
        receiver.HoldEnded += HandleHoldEnded;
        receiver.ChordTapped += HandleChordTapped;
    }

    void OnDisable()
    {
        receiver.SingleTapped -= HandleSingleTapped;
        receiver.HoldStarted -= HandleHoldStarted;
        receiver.HoldEnded -= HandleHoldEnded;
        receiver.ChordTapped -= HandleChordTapped;
    }

    void HandleSingleTapped(PressInfo pressInfo)
    {
        if (IsJumping())
        {
            Debug.Log("Tap during jump at " + GetProgressAtPress(pressInfo).ToString("0.000"));
        }
        else
        {
            StartJump();
        }
    }

    void HandleHoldStarted(PressInfo pressInfo)
    {
        if (IsJumping())
        {
            Debug.Log("Hold started during jump at " + GetProgressAtPress(pressInfo).ToString("0.000"));
        }
        else
        {
            StartJump();
        }
    }

    void HandleHoldEnded(PressInfo pressInfo)
    {
        if (IsJumping())
        {
            float progressAtRelease = GetProgressAtInputTime(pressInfo.Time);
            Debug.Log("Hold released at " + progressAtRelease.ToString("0.000"));
        }
    }

    void HandleChordTapped(PressInfo pressInfo)
    {
        Debug.Log("Chord " + string.Join("+", pressInfo.Buttons));
    }

    void StartJump()
    {
        jumpSequence = transform.DOJump(transform.position + jumpOffset, jumpPower, 1, jumpDuration);
    }

    bool IsJumping()
    {
        return jumpSequence != null && jumpSequence.IsActive() && jumpSequence.IsPlaying();
    }
    
    float GetProgressAtPress(PressInfo pressInfo)
    {
        return GetProgressAtInputTime(pressInfo.PressTime);
    }
    
    float GetProgressAtInputTime(double inputTime)
    {
        double secondsAgo = InputState.currentTime - inputTime;
        float elapsedAtEvent = jumpSequence.Elapsed() - (float)secondsAgo;
        return elapsedAtEvent / jumpSequence.Duration();
    }
}