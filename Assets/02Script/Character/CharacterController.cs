using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStateDC
{
    Idle,
    Ready,
    Run,
    Attack,
    TakeDamage,
    Die
}

public class CharacterController : MonoBehaviour
{
    protected CharacterAnimationController anim;
    private SpriteRenderer render;

    private CharacterStateDC state;
    public CharacterStateDC State
    {
        get => state;
    }

    private Direction look;
    public Direction Look
    {
        set
        {
            look = value;
            if (value == Direction.Left)
                render.flipX = true;
            else
                render.flipX = false;
        }
        get => look;
    }


    protected virtual void Awake()
    {
        if(!TryGetComponent<CharacterAnimationController>(out anim))
        {
            Debug.Log("CharacterContorller - Awake - AnimationController");
        }    
        if(!transform.GetChild(0).TryGetComponent<SpriteRenderer>(out render))
        {
            Debug.Log("CharacterController - Awkae - SpriteRenderer");
        }
        state = CharacterStateDC.Idle;
        look = Direction.Left;
    }

    public void PlayAnimation(CharacterStateDC state)
    {
        /*       if(this.state != state)
               {
                   this.state = state;
                   switch (state)
                   {
                       case CharacterStateDC.Idle:
                           anim.SetRun(false);
                           anim.SetReady(false);
                           break;
                       case CharacterStateDC.Ready:
                           anim.SetReady(true);
                           break;
                       case CharacterStateDC.Run:
                           anim.SetRun(true);
                           break;
                       case CharacterStateDC.Attack:
                           break;
                       case CharacterStateDC.TakeDamage:
                           anim.TakeDamage();
                           break;
                       case CharacterStateDC.Die:
                           anim.Die();
                           break;
                   }
               }
        */
    }
}
