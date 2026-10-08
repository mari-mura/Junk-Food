using DG.Tweening;
using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class DoJumpTestNew : MonoBehaviour
{
    [SerializeField] private TapHoldDetector detector;

    [SerializeField] private NodePath path;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private float jumpDuration = 0.8f;

    private JumpTimingTracker timing = new JumpTimingTracker();
    private JumpPoint pendingPoint;

    private bool CanJump => !timing.IsJumping && pendingPoint == null;

    private void Start()
    {
        path.ResetToStart();
        transform.position = path.CurrentPosition;
    }

    private void OnEnable()
    {
        detector.TapReleased += HandleTap;
        detector.HoldStarted += HandleHold;
    }

    private void OnDisable()
    {
        detector.TapReleased -= HandleTap;
        detector.HoldStarted -= HandleHold;
    }

    private void HandleTap(PressInfo pressInfo)
    {
        if (!CanJump) return;
        Debug.Log("Started Short Jump");
        StartJump(1, jumpDuration);
    }

    private void HandleHold(PressInfo pressInfo)
    {
        if (!CanJump) return;
        Debug.Log("Started Long Jump");
        StartJump(2, 2f);
    }

    private void StartJump(int steps, float duration)
    {
        Vector3 target = path.Advance(steps);
        JumpPoint point = path.CurrentPoint;

        timing = new JumpTimingTracker();
        Sequence jump = transform.DOJump(target, jumpPower, 1, duration);
        timing.Track(jump);

        if (point == null)
        {
            Debug.LogWarning("Destination has no JumpPoint component, skipping timing check");
            return;
        }

        pendingPoint = point;
        point.Succeeded += OnPointSucceeded;
        point.Failed += OnPointFailed;
        point.Activate(timing, detector);
    }

    private void OnPointSucceeded(JumpPoint point)
    {
        Debug.Log($"Good ({point.LastOffset:+0.000;-0.000}s)");
        Resolve(point);
    }

    private void OnPointFailed(JumpPoint point)
    {
        Debug.Log("Missed the timing");
        Resolve(point);
    }

    private void Resolve(JumpPoint point)
    {
        point.Succeeded -= OnPointSucceeded;
        point.Failed -= OnPointFailed;
        pendingPoint = null;
    }
}