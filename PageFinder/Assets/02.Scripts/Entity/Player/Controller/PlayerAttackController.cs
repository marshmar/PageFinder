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

    private float attackRange;
    private bool isAttacking;
    public float AttackRange { get { return attackRange; } }
    #endregion


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        attackSpeed = 1.0f;
        attackRange = 2.6f;
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
        base.SetTargetObject(false);
        if (isAttacking) yield return null;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        switch (attackType)
        {
            case AttackType.SHORTATTCK:
                // 범위 내에서 가장 가까운 적 찾기(없을 경우 공격 모션만)
                anim.SetTrigger("Attack");
                Debug.Log("가까운 적 공격");
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
                isAttacking = true;
                if (attackEnemy != null)
                { 
                    // 적 방향으로 플레이어 회전
                    TurnToDirection(CaculateDirection(attackEnemy));
                    attackEnemy.GetComponent<Enemy>().HP -= atk;
                }
                yield return new WaitForSeconds(animationLength);
                isAttacking = false;
                break;
            case AttackType.LONGATTACK:
                attackEnemy = base.TargetObject.GetComponent<attackTarget>().GetClosestEnemy();
                if (attackEnemy == null)
                {
                    yield return null;
                }
                else
                {
                    isAttacking = true;
                    anim.SetTrigger("Attack");
                    // 적 방향으로 플레이어 회전
                    TurnToDirection(CaculateDirection(attackEnemy));
                    attackEnemy.GetComponent<Enemy>().HP -= atk;
                    isAttacking = true;
                    yield return new WaitForSeconds(animationLength);
                    isAttacking = false;
                }
                break;
        }
    }

/*    public override void OnTargeting(Vector3 attackDir, float targetingRange)
    {
        base.OnTargeting(attackDir, targetingRange);
    }*/
}
