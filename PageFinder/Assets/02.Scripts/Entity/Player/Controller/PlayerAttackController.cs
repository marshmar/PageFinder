using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


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



    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    

    // Start is called before the first frame update
    public void Start()
    {
        attackEnemy = null;
        IsAttacking = false;

        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        currAnimationLength = 0.667f / playerScr.AttackSpeed;
        attackDelay = new WaitForSeconds(currAnimationLength);
        utilsManager = UtilsManager.Instance;

        inputTime = new WaitForSeconds(0.05f);
        comboCount = 0;
        comboCoroutine = null;
    }

    public void Update()
    {
    }
    public void Attack()
    {
        playerScr.Anim.SetTrigger("Attack");
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
            switch (comboCount)
            {
                case 0:
                    StartCoroutine(MoveAttack(-45.0f, 90.0f));
                    break;
                case 1:
                    StartCoroutine(MoveAttack(45.0f, -90.0f));
                    break;
                case 2:
                    StartCoroutine(MoveAttack(-70.0f, 140.0f));
                    break;
                default:
                    break;
            }

            yield return attackDelay;

            IsAttacking = false;


            yield break;
        }
        IsAttacking = true;
        playerScr.Anim.SetTrigger("Attack");

        switch (comboCount)
        {
            case 0:
                StartCoroutine(MoveAttack(-45.0f, 90.0f));
                break;
            case 1:
                StartCoroutine(MoveAttack(45.0f, -90.0f));
                break;
            case 2:
                StartCoroutine(MoveAttack(-70.0f, 140.0f));
                break;
            default:
                break;
        }
        Vector3 enemyDir = playerScr.CalculateDirection(attackEnemy);
        playerScr.TurnToDirection(enemyDir); // 적 방향으로 플레이어 회전
        StartCoroutine(MoveAttack(-45.0f, 90.0f));

        yield return attackDelay;
        
        IsAttacking = false;
    }


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
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / currAnimationLength-0.01f);

            attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + currDegree, 0);
            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        IsAttacking = false;
        yield break;
    }
    public IEnumerator Test()
    {
        Debug.Log("공격 끝1");
        isAbleComboAttack = false;
        comboCount = 2;

        yield return attackDelay;

    }

    public IEnumerator Test2()
    {

        Debug.Log("공격 끝2");
        isAbleComboAttack = false;
        comboCount = 0;

        yield return attackDelay;
        
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
        float checkTime = 0.3f;
        while (currTime <= checkTime)
        {
            if(isAbleComboAttack)
            {
                comboCount++;
                if (comboCount > 2)
                    comboCount = 0;
                isAbleComboAttack = false;
                StartCoroutine(AttackCoroutine());
                yield break;
            }
            currTime += Time.deltaTime;
            yield return null;

        }
        comboCount = 0;
    }

    public IEnumerator CheckCombo()
    {
        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
    }
}
