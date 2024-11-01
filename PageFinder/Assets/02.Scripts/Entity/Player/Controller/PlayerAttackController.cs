using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;


public class PlayerAttackController : MonoBehaviour
{
    #region Variable

    // 공격할 적 객체
    private Collider attackEnemy;

    private int comboCount;
    private bool isAttacking;
    private bool isAbleAttack;
    private float currAnimationLength;
    private WaitForSeconds attackDelay;
    private Player playerScr;
    private UtilsManager utilsManager;
    [SerializeField]
    private GameObject attackObj;
    [SerializeField]
    private GameObject targetObject;
    private PlayerTarget playerTargetScr;
    private TargetObject targetObjectScr;

    #endregion



    public bool IsAttacking { get => isAttacking; set { 
            isAttacking = value;
            if(!isAttacking)
                StartCoroutine(AttackDelayCoroutine());
            targetObjectScr.IsActive = false;
        } 
    }
    public int ComboCount { get => comboCount; set 
        { 
            comboCount = value;
            if (comboCount > 2) comboCount = 0;
        } 
    }

    public GameObject TargetObject { get => targetObject; set => targetObject = value; }
   



    // Start is called before the first frame update
    public void Start()
    {
        attackEnemy = null;

        isAttacking = false;

        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        playerTargetScr = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");
        targetObjectScr = DebugUtils.GetComponentWithErrorLogging<TargetObject>(this.gameObject, "TargetObject");
        currAnimationLength = 0.667f * 0.75f;
        attackDelay = new WaitForSeconds(playerScr.AttackSpeed);
        utilsManager = UtilsManager.Instance;

        comboCount = 0;
        attackObj.SetActive(false);
    }

    public IEnumerator AttackDelayCoroutine()
    {
        isAbleAttack = false;
        
        yield return attackDelay;

        isAbleAttack = true;
    }
    public void Attack()
    {
        if (!isAbleAttack) return;

        SetAttackEnemy();
        
        if(!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject)){
            playerTargetScr.CircleRangeOn(playerScr.AttackRange, 0.1f);
        }

        if (attackEnemy != null)
        {
            Vector3 enemyDir = playerScr.CalculateDirection(attackEnemy);
            targetObjectScr.IsActive = true;
            targetObjectScr.TargetTransform = attackEnemy.transform;

            IsAttacking = true;
            playerScr.TurnToDirection(enemyDir); // 적 방향으로 플레이어 회전
            playerScr.Anim.SetTrigger("Attack");
            
        }
        else
        {
            //targetObject.SetActive(false);
        }
    }

    public void DamageToEnemyEachComboStep()
    {
        switch (ComboCount)
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
    }
    
    public void SetAttackEnemy()
    {
        attackEnemy = utilsManager.FindMinDistanceObject(playerScr.Tr.position, playerScr.AttackRange+0.1f, 1 << 6);
    }

    public IEnumerator MoveAttack(float startDegree, float degreeAmount)
    {
        attackObj.SetActive(true);

        float attackTime = 0;
        float currDegree = startDegree;
        float targetDegree = startDegree + degreeAmount;

        attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + startDegree, 0);
        while (attackTime <= currAnimationLength - 0.2f)
        {
            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / (currAnimationLength-0.2f));

            attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + currDegree, 0);
            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        attackObj.SetActive(false);
        yield break;
    }
/*

    public IEnumerator Test()
    {
        Debug.Log("공격 끝1");
        isAbleComboAttack = false;
        ComboCount = 2;

        yield return attackDelay;

    }

    public IEnumerator Test2()
    {

        Debug.Log("공격 끝2");
        isAbleComboAttack = false;
        ComboCount = 0;

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

    public IEnumerator CheckCombo()
    {
        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
    }*/
}
