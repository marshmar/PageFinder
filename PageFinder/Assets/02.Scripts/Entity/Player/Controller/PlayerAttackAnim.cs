using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAnim : StateMachineBehaviour
{
    private PlayerAttackController playerAttackController;
    private PlayerMoveController playerMoveController;
    private PlayerAttackController _playerAttackController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerAttackController = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(animator.gameObject, "NewPlayerAttackController");

        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerAttackController>(_playerAttackController))
        {
            if (_playerAttackController.IsAnimatedBasedAttack())
            {
                _playerAttackController.IsNextAttackBuffered = false;
                _playerAttackController.ExcuteBehaviour();
            }
        }

        playerMoveController = DebugUtils.GetComponentWithErrorLogging<PlayerMoveController>(animator.gameObject, "PlayerMoveControllerScr");

        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerMoveController>(playerMoveController))
        {
            playerMoveController.CanTurn = false;
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
                playerMoveController.CanTurn = true;
                playerMoveController.CanMove = true;
            }
        }
    }
}
