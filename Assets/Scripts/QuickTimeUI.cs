using UnityEngine;

public class QuickTimeUI : MonoBehaviour
{
    public Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ButtonPressedAnim()
    {
        animator.SetBool("isHeld", true);
    }

    public void ButtonMissedAnim()
    {
        animator.SetTrigger("fail");
        ButtonDefaultAnim();
    }

    public void ButtonWinAnim()
    {
        animator.SetTrigger("win");
        ButtonDefaultAnim();
    }

    public void ButtonDefaultAnim()
    {
        animator.SetBool("isWalking", false);
    }
}
