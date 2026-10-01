using DG.Tweening;
using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class DoJumpTestNew : MonoBehaviour
{
    [SerializeField] private TapHoldDetector detector;
    [SerializeField] private NodePath path;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private float jumpDuration = 0.8f;

    private readonly JumpTimingTracker timing = new JumpTimingTracker();

    private void Start()
    {
        path.ResetToStart();
        transform.position = path.CurrentPosition;
    }

    private void OnEnable()
    {
        detector.ButtonDown += HandleButtonDown;
        detector.TapReleased += HandleTap;
        detector.HoldEnded += HandleReleased;
        detector.HoldStarted += HandleHold;
    }

    private void OnDisable()
    {
        detector.ButtonDown -= HandleButtonDown;
        detector.TapReleased -= HandleTap;
        detector.HoldEnded -= HandleReleased;
        detector.HoldStarted -= HandleHold;
    }

    private void HandleTap(PressInfo pressInfo)
    {
        if (!timing.IsJumping)
        {
            Debug.Log("Started Short Jump");
            StartJump(1, jumpDuration);
        }
    }

    private void HandleHold(PressInfo pressInfo)
    {
        if (!timing.IsJumping)
        {
            Debug.Log("Started Long Jump");
            StartJump(2, 2f);
        }
    }

    private void HandleButtonDown(string buttonName, double time)
    {
        timing.ReportRelease();
    }

    private void HandleReleased(PressInfo pressInfo)
    {
        timing.ReportRelease();
    }

    private void StartJump(int steps, float duration)
    {
        Vector3 target = path.Advance(steps);
        Sequence jump = transform.DOJump(target, jumpPower, 1, duration);
        timing.Track(jump);
    }
}