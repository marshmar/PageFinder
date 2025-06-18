using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAnim : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAttackController playerAttackController = animator.GetComponentSafe<PlayerAttackController>();
        if (playerAttackController.IsNull()) return;

        playerAttackController.IsAttacking = false;
        playerAttackController.IsNextAttackBuffered = false;
        playerAttackController.ComboCount = 0;
    }
}
