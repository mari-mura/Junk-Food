using DG.Tweening;
using UnityEngine;
public class JumpTimingTracker
{
    private Sequence jumpSequence;
    private bool hasLanded;
    private float landingTime;

    public bool IsJumping =>
        jumpSequence != null && jumpSequence.IsActive() && jumpSequence.IsPlaying();

    public void Track(Sequence sequence)
    {
        hasLanded = false;
        jumpSequence = sequence;
        jumpSequence.OnComplete(HandleLanded);
    }
    
    public void ReportReleaseOld()
    {
        if (jumpSequence == null)
        {
            return;
        }

        if (IsJumping)
        {
            float secondsBeforeLanding = jumpSequence.Duration() - jumpSequence.Elapsed();
            Debug.Log("Released " + secondsBeforeLanding.ToString("0.000") + "s before landing");
        }
        else if (hasLanded)
        {
            float secondsAfterLanding = Time.time - landingTime;
            Debug.Log("Released " + secondsAfterLanding.ToString("0.000") + "s after landing");
        }
        else
        {
            Debug.Log("Perfect jump");
        }
    }

    private void HandleLanded()
    {
        hasLanded = true;
        landingTime = Time.time;
    }
    
    public float GetOffset()
    {
        if (jumpSequence == null) return 0f;

        if (IsJumping)
            return -(jumpSequence.Duration() - jumpSequence.Elapsed()); // before landing
        if (hasLanded)
            return Time.time - landingTime;                              // after landing
        return 0f;
    }

    public void ReportRelease()
    {
        float offset = GetOffset();
        Debug.Log(offset < 0 ? $"Released {-offset:0.000}s before landing"
            : $"Released {offset:0.000}s after landing");
    }
}