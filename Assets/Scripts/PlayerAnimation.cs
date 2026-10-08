using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Sammy's script
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    // Need movement script or screen scroller to determine if player anim is walking
    // there's a bool in Animator named "isWalking" that determines the transition between
    //  idle anim & walking anim
    void Update()
    {
        //animator.SetBool("isWalking"), ;
    }
}
