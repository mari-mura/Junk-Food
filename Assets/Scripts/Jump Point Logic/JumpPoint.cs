using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


// TLDR: This is the parent class for all of the different types of nodes we have in the game, all of them will inherit from and override as such
public abstract class JumpPoint : PathPoint
{
    public event Action<JumpPoint> Succeeded;
    public event Action<JumpPoint> Failed;
    
    [SerializeField] private string promptOverride;

    protected virtual string DefaultPrompt => "";

    public string Prompt => string.IsNullOrEmpty(promptOverride) ? DefaultPrompt : promptOverride;

    // Basic value for how long the window is (value in both directions) in order for the player's input to be considered "good"
    [SerializeField] protected float timingWindow = 0.15f;

    protected JumpTimingTracker tracker;
    protected TapHoldDetector detector;
    public float LastOffset { get; private set; }
    private bool active;

    // This method fully links up the other components when the point is "activated" or read
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