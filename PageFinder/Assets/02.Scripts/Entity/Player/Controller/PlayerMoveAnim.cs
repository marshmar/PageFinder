using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAnim : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAttackController playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(animator.gameObject, "PlayerAttackController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerAttackController>(playerAttackControllerScr))
        {
            playerAttackControllerScr.IsAttacking = false;
            playerAttackControllerScr.ComboCount = 0;
            //playerAttackControllerScr.TargetObject.SetActive(false);
        }
/*        PlayerInkMagicController playerInkMagicControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerInkMagicController>(animator.gameObject, "PlayerInkMagicController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerInkMagicController>(playerInkMagicControllerScr))
        {
            playerInkMagicControllerScr.IsUsingInkMagic = false;
            //playerAttackControllerScr.TargetObject.SetActive(false);
        }*/
    }
}
