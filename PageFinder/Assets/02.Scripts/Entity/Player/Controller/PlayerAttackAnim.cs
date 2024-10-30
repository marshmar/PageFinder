using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAnim : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAttackController playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(animator.gameObject, "PlayerAttackController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerAttackController>(playerAttackControllerScr))
        {
            playerAttackControllerScr.IsAttacking = true;
            playerAttackControllerScr.DamageToEnemyEachComboStep();
            playerAttackControllerScr.ComboCount += 1;
        }
    }
}
