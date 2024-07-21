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

    public float AttackRange { get { return attackRange; } }
    #endregion


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        attackRange = 2.6f;
        attackEnemy = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 공격 타입에 따른 공격 실행
    /// </summary>
    /// <param name="attackType">공격 타입</param>
    public void OnAttack(AttackType attackType)
    {
        base.SetTargetObject(false);
        switch (attackType)
        {
            case AttackType.SHORTATTCK:
                // 범위 내에서 가장 가까운 적 찾기(없을 경우 공격 모션만)
                anim.SetTrigger("Attack");
                Debug.Log("가까운 적 공격");
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

/*    public override void OnTargeting(Vector3 attackDir, float targetingRange)
    {
        base.OnTargeting(attackDir, targetingRange);
    }*/
}
