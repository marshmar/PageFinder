using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackType
{
    SHORTATTCK = 0,
    LONGATTACK
}
public class PlayerAttackController : Player
{
    #region Attack

    // 공격할 적 객체
    Collider attackEnemy;


    private bool isAttacking;
    #endregion


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        attackEnemy = null;
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 공격 타입에 따른 공격 실행
    /// </summary>
    /// <param name="attackType">공격 타입</param>
    public IEnumerator OnAttack(AttackType attackType)
    {
        Debug.Log("공격 함수 진입");
        base.SetTargetObject(false);
        if (isAttacking) yield break;
        Debug.Log("공격 실행");
        isAttacking = true;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length / attackSpeed;

        switch (attackType)
        {
            case AttackType.SHORTATTCK:
                // 범위 내에서 가장 가까운 적 찾기(없을 경우 공격 모션만)
                anim.SetTrigger("Attack");
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
                if (attackEnemy != null)
                { 
                    // 적 방향으로 플레이어 회전
                    TurnToDirection(CaculateDirection(attackEnemy));
                    attackEnemy.GetComponent<Enemy>().HP -= atk;
                }
                yield return new WaitForSeconds(animationLength);
                break;
            case AttackType.LONGATTACK:
                attackEnemy = base.TargetObject.GetComponent<attackTarget>().GetClosestEnemy();
                if (attackEnemy == null)
                {
                    isAttacking = false;
                    yield break;
                }
                anim.SetTrigger("Attack");
                TurnToDirection(CaculateDirection(attackEnemy));
                attackEnemy.GetComponent<Enemy>().HP -= atk;
                yield return new WaitForSeconds(animationLength);
                break;
        }
        isAttacking = false;
    }

/*    public override void OnTargeting(Vector3 attackDir, float targetingRange)
    {
        base.OnTargeting(attackDir, targetingRange);
    }*/
}
