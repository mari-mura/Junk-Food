using UnityEngine;

// TLDR: SCRIPT FOR SAMMY!! Every method you should need is in here, this is where we'd add more if needed as well!
public class PlayerAnimationBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerJumpController controller;
    [SerializeField] private JumpMover mover;
    [SerializeField] private TapHoldReceiver input;

    // Subscribing to every event under the sun
    private void OnEnable()
    {
        controller.PointSucceeded += OnSucceed;
        controller.PointFailed += OnFail;
        mover.JumpStarted += OnJumpStarted;
        mover.Landed += OnLanded;
        input.HoldStarted += OnHoldStart;
        input.HoldEnded += OnHoldEnd;
    }

    // Unsubscribing from every event under the sun
    private void OnDisable()
    {
        controller.PointSucceeded -= OnSucceed;
        controller.PointFailed -= OnFail;
        mover.JumpStarted -= OnJumpStarted;
        mover.Landed -= OnLanded;
        input.HoldStarted -= OnHoldStart;
        input.HoldEnded -= OnHoldEnd;
    }

    // A jump just started
    private void OnJumpStarted(float duration)
    {
    }

    // The character just touched the ground on the next point
    private void OnLanded()
    {
    }

    // The player hit the timing window
    private void OnSucceed(JumpPoint point)
    {
    }

    // The player missed the timing window
    private void OnFail(JumpPoint point)
    {
    }

    // The player has the button held
    private void OnHoldStart(TapHoldDetector.PressInfo pressInfo)
    {
    }

    // The player released a hold
    private void OnHoldEnd(TapHoldDetector.PressInfo pressInfo)
    {
    }
}