using DG.Tweening;
using UnityEngine;
public class JumpTimingTracker
{
    private Sequence jumpSequence;
    private bool hasLanded;
    private float landingTime;

    public bool IsJumping => jumpSequence != null && jumpSequence.IsActive() && jumpSequence.IsPlaying();

    public void Track(Sequence sequence)
    {
        hasLanded = false;
        jumpSequence = sequence;
        jumpSequence.OnComplete(HandleLanded);
    }

    private void HandleLanded()
    {
        hasLanded = true;
        landingTime = Time.time;
    }
    
    public float GetOffset()
    {
        if (IsJumping)
        {
            return -(jumpSequence.Duration() - jumpSequence.Elapsed());
        }
        if (hasLanded)
        {
            return Time.time - landingTime;
        }
        return 0f;
    }

    public void ReportRelease()
    {
        float offset = GetOffset();
        Debug.Log(offset < 0 ? $"Released {-offset:0.000}s before landing" : $"Released {offset:0.000}s after landing");
    }
}