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
}

public class PlayerAttackController : MonoBehaviour
{
    #region Variable

    // 공격할 적 객체
    private Collider attackEnemy;

    private bool isAttacking;
    private float currAnimationLength;
    private WaitForSeconds attackDelay;
    private Player playerScr;
    private UtilsManager utilsManager;
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

    public GameObject Stel_BA_1Preafab;
    
    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        attackEnemy = null;
        isAttacking = false;
        currAnimationLength = 0.667f;
        basicAttackType = BasicAttackType.Stel_BA_1;
        attackDelay = new WaitForSeconds(currAnimationLength);
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        utilsManager = UtilsManager.Instance;
    }

    public void Attack()
    {
        if(!isAttacking)
            StartCoroutine(AttackCoroutine());
    }
    /// <summary>
    /// 공격 함수
    /// </summary>
    /// <returns></returns>
    public IEnumerator AttackCoroutine()
    {
        SetAttackEnemy();               // 공격 대상 설정
        if (attackEnemy == null)        // 공격 대상이 없을 경우
        {
            isAttacking = true;
            playerScr.Anim.SetTrigger("Attack");

            yield return attackDelay;

            isAttacking = false;
            yield break;
        }
        isAttacking = true;
        playerScr.Anim.SetTrigger("Attack");

        SetAttackDelay();                                // 공격 딜레이 설정
        Vector3 enemyDir = playerScr.CalculateDirection(attackEnemy);
        playerScr.TurnToDirection(enemyDir); // 적 방향으로 플레이어 회전
        GameObject attackObj = Instantiate(Stel_BA_1Preafab, new Vector3(playerScr.Tr.position.x, playerScr.Tr.position.y, playerScr.Tr.position.z), Quaternion.identity);
        if (attackObj)
        {
            if (attackObj.TryGetComponent<Stel_BA_1>(out Stel_BA_1 stel_BA_1))
            {
                stel_BA_1.EnemyTransform = attackEnemy.transform;
            }
        }
            
        yield return attackDelay;

        isAttacking = false;
    }

    public void SetAttackDelay()
    {
        AnimatorStateInfo stateInfo = playerScr.Anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length / playerScr.AttackSpeed;
        if(animationLength != currAnimationLength)
        {
            currAnimationLength = animationLength;
            attackDelay = new WaitForSeconds(currAnimationLength);
        }
    }


    public void SetAttackEnemy()
    {
        attackEnemy = utilsManager.FindMinDistanceObject(playerScr.Tr.position, playerScr.AttackRange, 1 << 6);
        if (attackEnemy == null) return;
    }
}
