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
    #endregion

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        attackEnemy = null;
        isAttacking = false;
        currAnimationLength = 0.667f;
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
            attackEnemy.GetComponent<Enemy>().HP -= atk;     // 데미지

            yield return attackDelay;

            isAttacking = false;
        }
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
                GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayAudioClip("CommonAttack");
                if (attackEnemy == null) return;
                break;
            case AttackType.LONGATTACK:
                attackEnemy = base.TargetObject.GetComponent<attackTarget>().GetClosestEnemy();
                GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayAudioClip("CommonAttack");
                break;
        }
        if (attackEnemy == null) return;

        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayAudioClip("Hit");
        Debug.Log("타겟팅 공격");
        anim.SetTrigger("Attack");
        // 적 방향으로 플레이어 회전
        TurnToDirection(CaculateDirection(attackEnemy));
         
        if(attackEnemy.CompareTag("ENEMY")) // 적 hp 감소
            attackEnemy.GetComponent<Enemy>().HP -= atk;
        else if (attackEnemy.CompareTag("OBJECT")) // 풍선의 색깔 변경
            attackEnemy.GetComponent<Balloon>().ChangeColor();
    }
}
