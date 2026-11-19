using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    [SerializeField] private PathFollower follower;

    [SerializeField] private TapHoldReceiver input;
    
    [SerializeField] private float moveSpeed = 0.05f;

    private Vector3 lastPosition;
    private bool isWalking = false;

    private void OnEnable()
    {
        PathSession.OnPointJudged += OnPointJudged;
        PathSession.OnPathFinished += OnPathFinished;

        if (input != null)
        {
            input.HoldStarted += OnHoldStart;
            input.HoldEnded += OnHoldEnd;
        }

        if (follower != null)
        {
            lastPosition = follower.transform.position;
        }
    }

    private void OnDisable()
    {
        PathSession.OnPointJudged -= OnPointJudged;
        PathSession.OnPathFinished -= OnPathFinished;

        if (input != null)
        {
            input.HoldStarted -= OnHoldStart;
            input.HoldEnded -= OnHoldEnd;
        }
    }
    
    private void LateUpdate()
    {
        if (follower == null)
        {
            return;
        }
        
        bool moving = false;
        if (Time.deltaTime > 0f)
        {
            float speed = (follower.transform.position - lastPosition).magnitude / Time.deltaTime;
            moving = speed > moveSpeed;
        }

        lastPosition = follower.transform.position;

        bool pathRunning = follower.Session != null && follower.Session.IsFinished == false;
        SetWalking(moving && pathRunning);
    }

    // A point on the path was judged (hit or miss)
    private void OnPointJudged(PathResult result)
    {
        if (result.Grade == HitGrade.Miss)
        {
            //quickTimeUI.ButtonMissedAnim();
            FailAnim();
        }
        else
        {
            //quickTimeUI.ButtonWinAnim();
        }
    }

    // The player reached the end of the path
    private void OnPathFinished()
    {
        IdleAnimStart();
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
        SetWalking(false);
    }

    // WALK
    void WalkAnimStart()
    {
        SetWalking(true);
    }

    // FAIL
    void FailAnim()
    {
        animator.Play("Fail");
    }
    
    private void SetWalking(bool walking)
    {
        if (walking == isWalking)
        {
            return;
        }

        isWalking = walking;
        animator.SetBool("isWalking", walking);
    }
}