using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    private UtilsManager utilsManager;
    [SerializeField]
    private GameObject attackObj;
    private PlayerTarget playerTargetScr;
    private TargetObject targetObjectScr;

    private PlayerDashController playerDashControllerScr;
    //private PlayerInkMagicController playerInkMagicControllerScr;
    private PlayerSkillController playerSkillControllerScr;

    private PlayerAnim playerAnim;
    private PlayerUtils playerUtils;
    private PlayerState playerState;

    #endregion



    public bool IsAttacking { get => isAttacking; set { 
            isAttacking = value;

            if (!isAttacking)
            {
                StartCoroutine(AttackDelayCoroutine());
                targetObjectScr.IsActive = false;
            }

        } 
    }
    public int ComboCount { get => comboCount; set 
        { 
            comboCount = value;
            if (comboCount > 2) comboCount = 0;
        } 
    }

    public WaitForSeconds AttackDelay { get => attackDelay; set => attackDelay = value; }



    public void Awake()
    {
        playerDashControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");
        //playerInkMagicControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerInkMagicController>(this.gameObject, "PlayerInkMagicController");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
    }

    // Start is called before the first frame update
    public void Start()
    {
        attackEnemy = null;

        isAttacking = false;

        playerTargetScr = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");
        targetObjectScr = DebugUtils.GetComponentWithErrorLogging<TargetObject>(this.gameObject, "TargetObject");
        currAnimationLength = 0.667f * 0.75f;

        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");

        attackDelay = new WaitForSeconds(playerState.DefaultAttackSpeed);
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
        if (!isAbleAttack || playerDashControllerScr.IsDashing  || playerSkillControllerScr.IsUsingSkill /*|| playerInkMagicControllerScr.IsUsingInkMagic*/) return;

        SetAttackEnemy();
        
        if(!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject)){
            playerTargetScr.CircleRangeOn(playerState.CurAttackRange, 0.1f);
        }

        if (attackEnemy != null)
        {
            Vector3 enemyDir = playerUtils.CalculateDirectionFromPlayer(attackEnemy);
            targetObjectScr.IsActive = true;
            targetObjectScr.TargetTransform = attackEnemy.transform;

            
            IsAttacking = true;
            playerUtils.TurnToDirection(enemyDir); // 적 방향으로 플레이어 회전
            playerAnim.SetAnimationTrigger("Attack");
            
        }
        else
        {
            //targetObject.SetActive(false);
        }
    }

    // 공격 콤보에 따라 다른 크기의 각도로 공격을 하는 함수
    public void SweepArkAttackEachComboStep()
    {
        switch (ComboCount)
        {
            case 0:
                StartCoroutine(SweepArkAttack(-45.0f, 90.0f));
                break;
            case 1:
                StartCoroutine(SweepArkAttack(45.0f, -90.0f));
                break;
            case 2:
                StartCoroutine(SweepArkAttack(-70.0f, 140.0f));
                break;
            default:
                break;
        }
    }
    
    public void SetAttackEnemy()
    {
        int targetLayer = (1 << 6) + (1 << 11);
        attackEnemy = utilsManager.FindMinDistanceObject(playerUtils.Tr.position, playerState.CurAttackRange+0.1f, targetLayer);
    }

    // 공격 오브젝트(투명 막대기)를 부채꼴 모양으로 움직이며 닿는 모든 적들에게 데미지를 입힌다.
    public IEnumerator SweepArkAttack(float startDegree, float degreeAmount)
    {
        attackObj.SetActive(true);

        float attackTime = 0;
        float currDegree = startDegree;
        float targetDegree = startDegree + degreeAmount;

        attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + startDegree, 0);
        while (attackTime <= currAnimationLength - 0.2f)
        {
            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / (currAnimationLength-0.1f));

            attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + currDegree, 0);
            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        attackObj.SetActive(false);
        yield break;
    }
}
