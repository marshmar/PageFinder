using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnim : MonoBehaviour
{
    #region Variables
    private Animator anim;
    [SerializeField] private AvatarMask upperBodyMask;
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        anim = this.GetComponentSafe<Animator>();
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    #endregion

    #region Getter
    public float GetAnimDuration()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }
    public Transform GetPlayerSpine()
    {
        return anim.GetBoneTransform(HumanBodyBones.Spine);
    }
    #endregion

    #region Setter
    public void SetAnimationTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void SetAnimationFloat(string animName, float value)
    {
        anim.SetFloat(animName, value);
        //anim.Update(0);
    }

    public void SetAnimationInteger(string animName, int value)
    {
        anim.SetInteger(animName, value);
    }


    public void SetLayerWeight(int layerIndex, float weight)
    {
        anim.SetLayerWeight(layerIndex, weight);
    }
    #endregion

    #region Utilities
    public bool HasAnimPassedTime(string animName, float timeThreshold)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= timeThreshold)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAttackAnimPassedTime(float timeThreshild)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.IsName("Player_Attack_1") || stateInfo.IsName("Player_Attack_2") || 
            stateInfo.IsName("Player_Attack_3")) && stateInfo.normalizedTime >= timeThreshild;
    }

    /// <summary>
    /// Returns the actual duration of the animation with speed applied
    /// </summary>
    /// <returns>actual duration of the animation with speed applied</returns>
    public float GetAdjustedAnimDuration()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float adjustedDuration = info.length / info.speed;

        return adjustedDuration;
    }

    public void ResetAnim()
    {
        anim.Rebind();
        anim.Update(0f);
    }
    #endregion

    #region Events
    #endregion
}
