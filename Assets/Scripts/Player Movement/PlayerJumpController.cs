using System;
using Unity.Tutorials.Editor;
using Unity.VisualScripting;
using UnityEngine;

//TLDR: Used to be DOJumpTestNew, now functions as the brain for all player movement. Listens for input, moves along the path, tells mover to jump

public class PlayerJumpController : MonoBehaviour
{
    [SerializeField] private TapHoldReceiver input;
    [SerializeField] private JumpMover mover;
    [SerializeField] private NodePath path;
    [SerializeField] private PromptDisplay promptDisplay;

    [SerializeField] private float holdJumpDuration = 2f;
    [SerializeField] private float tapJumpDuration = 0.8f;

    public event Action<JumpPoint> PointSucceeded;
    public event Action<JumpPoint> PointFailed;
    
    private JumpPoint pendingPoint;

    private bool CanJump => !mover.IsJumping && pendingPoint == null;

    private void Start()
    {
        path.ResetToStart();
        mover.SnapTo(path.CurrentPosition);
    }

    // EVENT SUBSCRIPTIONS
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

    
    // INPUT LOGIC 
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
    
    // "JUMPING" LOGIC
    private void StartJump(float duration)
    {
        Vector3 target = path.Advance(1);
        JumpPoint point = path.CurrentPoint;

        // Lowkey this is mainly for me bc I keep forgetting to test with jump point prefabs
        if (point == null)
        {
            Debug.LogWarning("Node next in line does not have a JumpPoint component");
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

    // HOW GOOD DID THEY JUMP
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