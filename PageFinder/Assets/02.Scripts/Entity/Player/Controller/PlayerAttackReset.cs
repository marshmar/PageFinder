using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackReset : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAttackController playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(animator.gameObject, "PlayerAttackController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerAttackController>(playerAttackControllerScr))
        {
            Debug.Log(playerAttackControllerScr.IsAttacking);
            playerAttackControllerScr.IsAttacking = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAttackController playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(animator.gameObject, "PlayerAttackController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerAttackController>(playerAttackControllerScr))
        {
            Debug.Log(playerAttackControllerScr.IsAttacking);
            playerAttackControllerScr.IsAttacking = false;
        }
    }
}
