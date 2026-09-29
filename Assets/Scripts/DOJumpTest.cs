using DG.Tweening;
using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class DoJumpTest : MonoBehaviour
{
    [SerializeField] private TapHoldDetector detector;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private float jumpDuration = 0.8f;

    private Sequence jumpSequence;
    private bool hasLanded;
    private float landingTime;

    private void OnEnable()
    {
        detector.ButtonDown += HandleButtonDown;
        detector.TapReleased += HandleReleased;
        detector.HoldEnded += HandleReleased;
    }

    private void OnDisable()
    {
        detector.ButtonDown -= HandleButtonDown;
        detector.TapReleased -= HandleReleased;
        detector.HoldEnded -= HandleReleased;
    }

    private void HandleButtonDown(string buttonName, double time)
    {
        if (!IsJumping())
        {
            StartJump();
        }
    }

    private void HandleReleased(PressInfo pressInfo)
    {
        if (jumpSequence == null)
        {
            return;
        }

        if (IsJumping())
        {
            ReportEarlyRelease();
        }
        else if (hasLanded)
        {
            ReportLateRelease();
        }
    }

    private void StartJump()
    {
        Vector3 targetPosition = GetOtherPoint();
        hasLanded = false;
        jumpSequence = transform.DOJump(targetPosition, jumpPower, 1, jumpDuration);
        jumpSequence.OnComplete(HandleLanded);
    }

    private void HandleLanded()
    {
        hasLanded = true;
        landingTime = Time.time;
    }

    private Vector3 GetOtherPoint()
    {
        float distanceToLeft = Vector3.Distance(transform.position, leftPoint.position);
        float distanceToRight = Vector3.Distance(transform.position, rightPoint.position);

        if (distanceToLeft < distanceToRight)
        {
            return rightPoint.position;
        }

        return leftPoint.position;
    }

    private void ReportEarlyRelease()
    {
        float secondsBeforeLanding = jumpSequence.Duration() - jumpSequence.Elapsed();
        Debug.Log("Released " + secondsBeforeLanding.ToString("0.000") + "s before landing");
    }

    private void ReportLateRelease()
    {
        float secondsAfterLanding = Time.time - landingTime;
        Debug.Log("Released " + secondsAfterLanding.ToString("0.000") + "s after landing");
    }

    private bool IsJumping()
    {
        return jumpSequence != null && jumpSequence.IsActive() && jumpSequence.IsPlaying();
    }
}