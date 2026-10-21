using DG.Tweening;
using TMPro;
using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class DoJumpTestNew : MonoBehaviour
{
    [SerializeField] private TapHoldDetector detector;

    [SerializeField] private NodePath path;
    [SerializeField] private float jumpPower = 0.01f;
    [SerializeField] private float jumpDuration = 0.8f;
    [SerializeField] private TMP_Text promptText;

    private JumpTimingTracker timing = new JumpTimingTracker();
    private JumpPoint pendingPoint;

    private bool CanJump => !timing.IsJumping && pendingPoint == null;
    
    // Sam: for animation
    public QuickTimeUI quickTimeUI;
    public bool isWalking = false;
    public event System.Action onFail;

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
        StartJump(1, jumpDuration);
    }

    private void HandleHold(PressInfo pressInfo)
    {
        if (!CanJump) return;
        StartJump(1, 2f);
        
    }

    private void StartJump(int steps, float duration)
    {
        isWalking = true;
        Vector3 target = path.Advance(steps);
        JumpPoint point = path.CurrentPoint;

        if (promptText != null)
        {
            promptText.text = point.Prompt;
        }

        timing = new JumpTimingTracker();
        Sequence jump = transform.DOJump(target, jumpPower, 1, duration);
        timing.Track(jump);
        
        pendingPoint = point;
        point.Succeeded += OnPointSucceeded;
        point.Failed += OnPointFailed;
        point.Activate(timing, detector);
    }

    private void OnPointSucceeded(JumpPoint point)
    {
        Debug.Log($"Good ({point.LastOffset:+0.000;-0.000}s)");
        quickTimeUI.ButtonWinAnim();
        Resolve(point);
    }

    private void OnPointFailed(JumpPoint point)
    {
        Debug.Log("Missed");
        quickTimeUI.ButtonMissedAnim();
        StartFailedAnim();
        Resolve(point);
        
    }

    private void Resolve(JumpPoint point)
    {
        point.Succeeded -= OnPointSucceeded;
        point.Failed -= OnPointFailed;
        pendingPoint = null;
    }

    // Sam: animation triggers
    public void EndWalkAnim()
    {
        if (timing.hasLanded)
        {
            isWalking = false;
        }
    }

    public void StartFailedAnim()
    {
        onFail?.Invoke();
    }
    
}