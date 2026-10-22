using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    //[SerializeField] private QuickTimeUI quickTimeUI;
    
    [SerializeField] private PlayerJumpController controller;
    [SerializeField] private Mover mover;
    [SerializeField] private TapHoldReceiver input;

    private void OnEnable()
    {
        controller.PointSucceeded += OnSucceed;
        controller.PointFailed += OnFail;
        mover.JumpStarted += OnJumpStarted;
        mover.Landed += OnLanded;
        input.HoldStarted += OnHoldStart;
        input.HoldEnded += OnHoldEnd;
    }

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
        WalkAnimStart();
    }

    // Touched down
    private void OnLanded()
    {
        IdleAnimStart();
    }

    // The player hit the timing window
    private void OnSucceed(JumpPoint point)
    {
        //quickTimeUI.ButtonWinAnim();
    }

    // The player missed the timing window.
    private void OnFail(JumpPoint point)
    {
        //quickTimeUI.ButtonMissedAnim();
        FailAnim();
    }

    // The button is held down
    private void OnHoldStart(TapHoldDetector.PressInfo pressInfo)
    {
        //quickTimeUI.ButtonPressedAnim();
    }

    // The hold is released
    private void OnHoldEnd(TapHoldDetector.PressInfo pressInfo)
    {
        //quickTimeUI.ButtonDefaultAnim();
    }
    
    // IDLE
    void IdleAnimStart()
    {
        animator.SetBool("isWalking", false);
    }

    // WALK
    void WalkAnimStart()
    {
        animator.SetBool("isWalking", true);
    }

    // FAIL
    void FailAnim()
    {
        animator.Play("Fail");
    }
}