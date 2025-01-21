using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnim : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = DebugUtils.GetComponentWithErrorLogging<Animator>(this.gameObject, "Animator");
    }
    
    public void CheckAnimProgress(string animName, float time, ref bool state)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= time)
            {
                state = false;
                return;
            }
            state = true;
        }
    }

    public void SetAnimationTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void SetAnimationFloat(string animName, float value)
    {
        anim.SetFloat(animName, value);
    }
}
