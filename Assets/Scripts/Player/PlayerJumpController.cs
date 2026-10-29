using System;
using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    [SerializeField] private TapHoldReceiver input;
    [SerializeField] private Mover mover;
    [SerializeField] private PromptDisplay promptDisplay;
    [SerializeField] private NodePath path;
    
    [SerializeField] private float tapJumpDuration = 0.8f;
    [SerializeField] private float holdJumpDuration = 2f;

    public event Action<JumpPoint> PointSucceeded;
    public event Action<JumpPoint> PointFailed;
    private JumpPoint pendingPoint;

    private bool CanJump => !mover.IsJumping && pendingPoint == null;
    
    private void Start()
    {
        path.ResetToStart();
        mover.SnapTo(path.CurrentPosition);
    }

    private void OnEnable()
    {
        input.SingleTapped += OnTap;
        input.ChordTapped += OnTap;
        input.HoldStarted += OnHold;
    }

    private void OnDisable()
    {
        input.SingleTapped -= OnTap;
        input.ChordTapped -= OnTap;
        input.HoldStarted -= OnHold;
    }

    // INPUT
    private void OnTap(TapHoldDetector.PressInfo pressInfo)
    {
        if (!CanJump) return;
        StartJump(tapJumpDuration);
    }

    private void OnHold(TapHoldDetector.PressInfo pressInfo)
    {
        if (!CanJump) return;
        StartJump(holdJumpDuration);
    }

    // JUMPING
    private void StartJump(float duration)
    {
        Vector3 target = path.Advance(1);
        JumpPoint point = path.CurrentPoint;

        if (point == null)
        {
            Debug.Log("Next node is not a JumpPoint");
        }

        if (promptDisplay != null)
        {
            promptDisplay.Show(point.Prompt);
        }
        
        mover.JumpTo(target, duration);
        
        pendingPoint = point;
        point.Succeeded += OnPointSucceeded;
        point.Failed += OnPointFailed;
        point.Activate(mover.Timing, input.Detector);
    }

    // HOW GOOD THEY DID
    private void OnPointSucceeded(JumpPoint point)
    {
        Debug.Log($"Good ({point.LastOffset:+0.000;-0.000}s)");
        Resolve(point);
        PointSucceeded?.Invoke(point);
    }

    private void OnPointFailed(JumpPoint point)
    {
        Debug.Log("Missed");
        Resolve(point);
        PointFailed?.Invoke(point);
    }

    private void Resolve(JumpPoint point)
    {
        point.Succeeded -= OnPointSucceeded;
        point.Failed -= OnPointFailed;
        pendingPoint = null;
    }

}