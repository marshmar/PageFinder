using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAnim : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        NewPlayerAttackController newPlayerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<NewPlayerAttackController>(animator.gameObject, "NewPlayerAttackController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<NewPlayerAttackController>(newPlayerAttackControllerScr))
        {
            newPlayerAttackControllerScr.IsAttacking = false;
            newPlayerAttackControllerScr.IsNextAttackBuffered = false;
            newPlayerAttackControllerScr.ComboCount = 0;
        }
    }
}
