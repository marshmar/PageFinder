using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAnim : StateMachineBehaviour
{
    private PlayerAttackController playerAttackController;
    private PlayerMoveController playerMoveController;
    private NewPlayerAttackController newPlayerAttackController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        newPlayerAttackController = DebugUtils.GetComponentWithErrorLogging<NewPlayerAttackController>(animator.gameObject, "NewPlayerAttackController");

        if (!DebugUtils.CheckIsNullWithErrorLogging<NewPlayerAttackController>(newPlayerAttackController))
        {
            if (newPlayerAttackController.IsAnimatedBasedAttack())
            {
                newPlayerAttackController.IsNextAttackBuffered = false;
                newPlayerAttackController.ExcuteBehaviour();
            }
        }

        playerMoveController = DebugUtils.GetComponentWithErrorLogging<PlayerMoveController>(animator.gameObject, "PlayerMoveControllerScr");

        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerMoveController>(playerMoveController))
        {
            playerMoveController.MoveTurn = false;
            playerMoveController.CanMove = false;
        }

/*        playerAttackController = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(animator.gameObject, "PlayerAttackController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerAttackController>(playerAttackController))
        {
            playerAttackController.SweepArkAttackEachComboStep();
            playerAttackController.ComboCount += 1;
        }*/


    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0.8f)
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerMoveController>(playerMoveController))
            {
                playerMoveController.MoveTurn = true;
                playerMoveController.CanMove = true;
            }
        }
    }
}
