using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnim : MonoBehaviour, IListener
{
    private Animator anim;

    [SerializeField] private AvatarMask upperBodyMask;
    private void Awake()
    {
        anim = DebugUtils.GetComponentWithErrorLogging<Animator>(this.gameObject, "Animator");
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.UI_Changed, this);
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

    public void SetAnimationInteger(string animName, int value)
    {
        anim.SetInteger(animName, value);
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        anim.SetLayerWeight(layerIndex, weight);
    }
    

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.UI_Changed:
                UIType uiType = (UIType)Param;
                if(uiType == UIType.Defeat)
                    SetAnimationTrigger("Die");
                break;
        }
    }
}
