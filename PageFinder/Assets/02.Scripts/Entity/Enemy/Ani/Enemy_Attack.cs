using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : StateMachineBehaviour
{
    bool isAttack = false;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // EnemyController가 아니라 상속받는 다른 클래스있기 때문에 그에 대처하는 코드 필요함.
        EnemyController enemyController = animator.gameObject.GetComponent<EnemyController>();

        // 나중에 공격 애니메이션 하는 중에 플레이어에게 데미지를 입힐 타이밍에 대한 데이터값에 따라 변경하도록 설정하기

        // 애니메이션이 절반 이상 동작했을 경우
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && !isAttack)
        {
            if (!enemyController.CheckIfPlayerIsWithInAttackRange())
                return;

            // 공격 범위 안에 플레이어가 있을 경우
            isAttack = true;
            enemyController.playerScr.HP -= enemyController.ATK * (enemyController.DefaultAtkPercent / 100);
            Debug.Log("플레이어 HP : " + enemyController.playerScr.HP);
            enemyController.state = EnemyController.State.IDLE;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemyController = animator.gameObject.GetComponent<EnemyController>();

        enemyController.CurrDefaultAtkCoolTime = enemyController.MaxDefaultAtkCoolTime;
        //enemyController.state = EnemyController.State.MOVE;
        isAttack = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
