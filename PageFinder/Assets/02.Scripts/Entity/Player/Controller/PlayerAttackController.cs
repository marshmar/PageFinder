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

    private int comboCount;
    private bool isAttacking;
    private bool isAbleComboAttack;
    private float currAnimationLength;
    private WaitForSeconds attackDelay;
    private Player playerScr;
    private UtilsManager utilsManager;
    [SerializeField]
    private GameObject attackObj;
    private WaitForSeconds inputTime;
    private Coroutine comboCoroutine;
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
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    public GameObject Stel_BA_1Preafab;
    
    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        attackEnemy = null;
        IsAttacking = false;
        currAnimationLength = 0.667f / 2;
        basicAttackType = BasicAttackType.Stel_BA_1;
        attackDelay = new WaitForSeconds(currAnimationLength);
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        utilsManager = UtilsManager.Instance;

        inputTime = new WaitForSeconds(0.1f);
        comboCount = 0;
        comboCoroutine = null;
    }

    public void Attack()
    {
        if (!IsAttacking)
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
            IsAttacking = true;
            playerScr.Anim.SetTrigger("Attack");

            StartCoroutine(MoveAttack(-45.0f, 90.0f));
            comboCount = 1;
            yield return attackDelay;

            if(comboCoroutine == null)
            {
                comboCount = 0;
                IsAttacking = false;
            }


            yield break;
        }
        IsAttacking = true;
        playerScr.Anim.SetTrigger("Attack");
            
        //SetAttackDelay();                                // 공격 딜레이 설정
        Vector3 enemyDir = playerScr.CalculateDirection(attackEnemy);
        playerScr.TurnToDirection(enemyDir); // 적 방향으로 플레이어 회전
        StartCoroutine(MoveAttack(-45.0f, 90.0f));
        /*GameObject attackObj = Instantiate(Stel_BA_1Preafab, new Vector3(playerScr.Tr.position.x, playerScr.Tr.position.y, playerScr.Tr.position.z), Quaternion.identity);
        if (attackObj)
        {
            if (attackObj.TryGetComponent<Stel_BA_1>(out Stel_BA_1 stel_BA_1))
            {
                stel_BA_1.EnemyTransform = attackEnemy.transform;
            }
        }*/

        comboCount = 1;
        yield return attackDelay;
        
        IsAttacking = false;
    }

/*    public void SetAttackDelay()
    {
        AnimatorStateInfo stateInfo = playerScr.Anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length / playerScr.AttackSpeed;
        if(animationLength != currAnimationLength)
        {
            currAnimationLength = animationLength;
            attackDelay = new WaitForSeconds(currAnimationLength);
        }
    }*/


    public void SetAttackEnemy()
    {
        attackEnemy = utilsManager.FindMinDistanceObject(playerScr.Tr.position, playerScr.AttackRange, 1 << 6);
        if (attackEnemy == null) return;
    }

    public IEnumerator MoveAttack(float startDegree, float degreeAmount)
    {
        float attackTime = 0;
        float currDegree = startDegree;
        float targetDegree = startDegree + degreeAmount;

        attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + startDegree, 0);
        while (attackTime <= currAnimationLength)
        {
            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / currAnimationLength);

            attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + currDegree, 0);
            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        IsAttacking = false;
        yield break;
    }
    public IEnumerator Test()
    {
        isAttacking = true;
        Debug.Log("공격 끝1");
        playerScr.Anim.SetTrigger("Attack2");
        isAbleComboAttack = false;
        comboCount = 2;

        yield return attackDelay;

        Debug.Log(comboCoroutine == null);
        if (comboCoroutine == null)
        {
            isAttacking = false;
            comboCount = 0;
        }
    }

    public IEnumerator Test2()
    {
        isAttacking = true;
        Debug.Log("공격 끝2");
        playerScr.Anim.SetTrigger("Attack3");
        isAbleComboAttack = false;
        comboCount = 0;

        yield return attackDelay;

        Debug.Log(comboCoroutine == null);
        if (comboCoroutine == null)
        {
            isAttacking = false;
            comboCount = 0;
        }
    }

    public void CheckInput()
    {
        if (isAbleComboAttack) {
            StopCoroutine(InputCheckCoroutine());
        }
        StartCoroutine(InputCheckCoroutine());
    }

    public IEnumerator InputCheckCoroutine()
    {
        isAbleComboAttack = true;

        yield return inputTime;

        isAbleComboAttack = false;
    }
    public IEnumerator ComboCoroutine()
    {
        float currTime = 0f;
        float checkTime = 0.7f;
        while (currTime <= checkTime)
        {
            if(isAbleComboAttack)
            {
                if(comboCount == 1)
                {
                    Debug.Log("TEST 실행");
                    comboCoroutine = StartCoroutine(Test());
                }
                else if(comboCount == 2)
                {
                    Debug.Log("TEST2 실행");
                    comboCoroutine = StartCoroutine(Test2());
                }
                yield break;
            }
            currTime += Time.deltaTime;
            yield return null;

        }
        comboCoroutine = null;
    }

}
