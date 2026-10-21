using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public DoJumpTestNew JumpTest;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        JumpTest = GetComponent<DoJumpTestNew>(); 
    }
    
    void Update()
    {
        JumpTest.EndWalkAnim();
        
        if (JumpTest.isWalking) // true
        {
            WalkAnimStart();
        }
        if (!JumpTest.isWalking) // false
        {
            IdleAnimStart();
        }
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
    void OnEnable()
    {
        JumpTest.onFail += FailAnim;   // subscribe to Fail
    }
    void OnDisable()
    {
        JumpTest.onFail -= FailAnim;   // unsubscribe to Fail
    }
    void FailAnim()
    {
        animator.Play("Fail");
    }

}
