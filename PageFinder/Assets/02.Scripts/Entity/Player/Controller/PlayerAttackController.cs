using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackType
{
    SHORTATTCK = 0,
    LONGATTACK
}

public enum BasicAttackType
{
    Stel_BA_1,  // 물감 방울 투사체 공격
    Stel_BA_2,  // 획 긋기 히트스캔 공격
    Stel_BA_3   // 물감 투하 투사체 공격
}

public class PlayerAttackController : Player
{
    #region Variable

    // 공격할 적 객체
    Collider attackEnemy;

    private bool isAttacking;
    float currAnimationLength;
    WaitForSeconds attackDelay;

    #endregion

    #region Property
    private AttackType attackType;
    public AttackType AttackType
    {
        get { return attackType; }
        set
        {
            if (attackType == value)
                return;

            attackType = value;
        }
    }

    private BasicAttackType basicAttackType;
    public BasicAttackType BasicAttackType { get => basicAttackType; set => basicAttackType = value; }


    
    #endregion

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        attackEnemy = null;
        isAttacking = false;
        currAnimationLength = 0.667f;
        basicAttackType = BasicAttackType.Stel_BA_1;
        attackDelay = new WaitForSeconds(currAnimationLength);
    }

    // Update is called once per frame
    void Update()
    {

    }



    /// <summary>
    /// 공격 함수
    /// </summary>
    /// <returns></returns>
    public IEnumerator Attack()
    {
        Debug.Log("공격 시작");
        lineRenderer.enabled = false;
        base.SetTargetObject(false);        // 타겟팅 오브젝트 비활성화
        if (!isAttacking)            // 공격중이 아니면
        {
            SetAttackEnemy();               // 공격 대상 설정
            if (attackEnemy == null)        // 공격 대상이 없을 경우
            {
                if(attackType == AttackType.SHORTATTCK) // 공격 방식이 짧은 공격이면 애니메이션 활성화
                {
                    isAttacking = true;
                    anim.SetTrigger("Attack");

                    yield return attackDelay;

                    isAttacking = false;
                }
                yield break;
            }

            isAttacking = true;
            anim.SetTrigger("Attack");

            SetAttackDelay();                                // 공격 딜레이 설정
            TurnToDirection(CaculateDirection(attackEnemy)); // 적 방향으로 플레이어 회전
            attackEnemy.GetComponent<EnemyAction>().HP = atk;     // 데미지

            yield return attackDelay;

            isAttacking = false;
        }
    }

    /// <summary>
    /// 타겟팅 객체 움직이기
    /// </summary>
    /// <param name="targetingRange">공격 범위</param>
    public override void OnTargeting(Vector3 attackDir, float targetingRange)
    {
        switch (basicAttackType)
        {
            case BasicAttackType.Stel_BA_1:
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, tr.position);
                lineRenderer.SetPosition(1, GetRayPosition(attackDir));
                break;
            case BasicAttackType.Stel_BA_2:
                break;
            case BasicAttackType.Stel_BA_3:
                SetTargetObject(true);

                // 사거리를 벗어날 경우 제자리 고정
                if (Vector3.Distance(tr.position, targetObjectTr.position) >= targetingRange)
                {
                    targetObjectTr.position = (tr.position - targetObjectTr.position).normalized * targetingRange;
                }
                // 타겟팅 오브젝트 움직이기
                else
                {
                    targetObjectTr.position = (tr.position + (attackDir) * (targetingRange - 0.1f));
                    targetObjectTr.position = new Vector3(targetObjectTr.position.x, 0.1f, targetObjectTr.position.z);
                }
                break;
        }

    }

    public Vector3 GetRayPosition(Vector3 dir)
    {
        Ray ray = new Ray(tr.position, dir.normalized);
        RaycastHit rayHit;
        if(Physics.Raycast(ray, out rayHit, 20.0f))
        {
            return rayHit.point;
        }
        return tr.position + dir.normalized * 20.0f;

    }
    public void SetAttackDelay()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length / attackSpeed;
        if(animationLength != currAnimationLength)
        {
            currAnimationLength = animationLength;
            attackDelay = new WaitForSeconds(currAnimationLength);
        }
    }


    public void SetAttackEnemy()
    {
        switch (attackType)
        {
            case AttackType.SHORTATTCK:
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
                if (attackEnemy == null) return;
                break;
            case AttackType.LONGATTACK:
                attackEnemy = base.TargetObject.GetComponent<attackTarget>().GetClosestEnemy();
                break;
        }
        
    }
}
