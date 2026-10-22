using System;
using DG.Tweening;
using UnityEngine;

// TLDR: Moves this object from Point A to Point B
public class Mover : MonoBehaviour
{
    [SerializeField] private float jumpPower = 0.01f;
    
    public event Action<float> JumpStarted;
    public event Action Landed;
    
    public JumpTimingTracker Timing { get; private set; } = new JumpTimingTracker();

    public bool IsJumping => Timing.IsJumping;

    public void SnapTo(Vector3 position)
    {
        transform.position = position;
    }

    public void JumpTo(Vector3 target, float duration)
    {
        Timing = new JumpTimingTracker();
        Timing.Landed += HandleLanded;

        Sequence jump = transform.DOJump(target, jumpPower, 1, duration);
        Timing.Track(jump);

        JumpStarted?.Invoke(duration);
    }

    private void HandleLanded()
    {
        Landed?.Invoke();
    }
}