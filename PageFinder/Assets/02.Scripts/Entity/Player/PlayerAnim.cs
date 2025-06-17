using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnim : MonoBehaviour
{
    #region Variables
    private Animator _anim;
    [SerializeField] private AvatarMask UpperBodyMask;
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _anim = this.GetComponentSafe<Animator>();
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    #endregion

    #region Getter
    public float GetAnimDuration()
    {
        return _anim.GetCurrentAnimatorStateInfo(0).length;
    }
    public Transform GetPlayerSpine()
    {
        return _anim.GetBoneTransform(HumanBodyBones.Spine);
    }
    #endregion

    #region Setter
    public void SetAnimationTrigger(string triggerName)
    {
        _anim.SetTrigger(triggerName);
    }

    public void SetAnimationFloat(string animName, float value)
    {
        _anim.SetFloat(animName, value);
        //anim.Update(0);
    }

    public void SetAnimationInteger(string animName, int value)
    {
        _anim.SetInteger(animName, value);
    }


    public void SetLayerWeight(int layerIndex, float weight)
    {
        _anim.SetLayerWeight(layerIndex, weight);
    }
    #endregion

    #region Utilities
    public bool HasAnimPassedTime(string animName, float timeThreshold)
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= timeThreshold)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAttackAnimPassedTime(float timeThreshild)
    {
        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.IsName("Player_Attack_1") || stateInfo.IsName("Player_Attack_2") || 
            stateInfo.IsName("Player_Attack_3")) && stateInfo.normalizedTime >= timeThreshild;
    }

    /// <summary>
    /// Returns the actual duration of the animation with speed applied
    /// </summary>
    /// <returns>actual duration of the animation with speed applied</returns>
    public float GetAdjustedAnimDuration()
    {
        AnimatorStateInfo info = _anim.GetCurrentAnimatorStateInfo(0);
        float adjustedDuration = info.length / info.speed;

        return adjustedDuration;
    }

    public void ResetAnim()
    {
        _anim.Rebind();
        _anim.Update(0f);
    }
    #endregion

    #region Events
    #endregion
}
