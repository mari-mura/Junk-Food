using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public abstract class JumpPoint : MonoBehaviour
{
    public event Action<JumpPoint> Succeeded;
    public event Action<JumpPoint> Failed;

    [SerializeField] protected float timingWindow = 0.15f;

    protected JumpTimingTracker tracker;
    protected TapHoldDetector detector;
    public float LastOffset { get; private set; }
    private bool active;

    public void Activate(JumpTimingTracker t, TapHoldDetector d)
    {
        tracker = t;
        detector = d;
        active = true;
        OnActivate();
    }

    public abstract void OnActivate();
    public abstract void OnDeactivate();
    
    public void Judge(double inputTime)
    {
        float latency = (float)(InputState.currentTime - inputTime);
        LastOffset = tracker.GetOffset() - latency;

        if (Mathf.Abs(LastOffset) <= timingWindow) Succeed(); else Fail();
    }

    public virtual void Update()
    {
        if (active && tracker.GetOffset() > timingWindow) Fail();
    }

    public void Succeed()
    {
        if (!active) return;
        End();
        Succeeded?.Invoke(this);
    }

    public void Fail()
    {
        if (!active) return;
        End();
        Failed?.Invoke(this);
    }

    private void End()
    {
        active = false;
        OnDeactivate();
    }
}