using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    private int H_Run = Animator.StringToHash("Run");
    private int H_TakeDamage = Animator.StringToHash("TakeDamage");
    private int H_Die = Animator.StringToHash("Die");
    private int H_Ready = Animator.StringToHash("Ready");
    private int H_Slash = Animator.StringToHash("Slash");
    private int H_Jab = Animator.StringToHash("Jab");
    private int H_Push = Animator.StringToHash("Push");
    private int H_Shot = Animator.StringToHash("Shot");


    private void Awake()
    {
        if (!TryGetComponent<Animator>(out animator))
        {
            Debug.Log("CharacterAnimationController - Awake - Animator");
        }
    }

    public void SetRun(bool isRuning)
    {
        animator.SetBool(H_Run, isRuning);
    }

    public void TakeDamage()
    {
        animator.SetTrigger(H_TakeDamage);
    }

    public void Die()
    {
        animator.SetTrigger(H_Die);
    }
    
    public void SetReady(bool isReady)
    {
        animator.SetBool(H_Ready, isReady);
    }

    public void Slash()
    {
        animator.SetTrigger(H_Slash);
    }

    public void Jab()
    {
        animator.SetTrigger(H_Jab);
    }

    public void Push()
    {
        animator.SetTrigger(H_Push);
    }

    public void Shot()
    {
        animator.SetTrigger(H_Shot);
    }

}

