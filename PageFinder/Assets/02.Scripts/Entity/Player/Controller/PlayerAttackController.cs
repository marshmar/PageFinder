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
    private Player playerScr;
    private UtilsManager utilsManager;
    [SerializeField]
    private GameObject attackObj;
    private PlayerTarget playerTargetScr;
    private TargetObject targetObjectScr;

    private PlayerDashController playerDashControllerScr;
    private PlayerInkMagicController playerInkMagicControllerScr;
    private PlayerSkillController playerSkillControllerScr;

    [SerializeField]
    private Sprite[] attackTypeImages;
    [SerializeField]
    private Image attackTypeImage;
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
        playerInkMagicControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerInkMagicController>(this.gameObject, "PlayerInkMagicController");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
    }

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

    public void SetAttackTypeImage(InkType inkType)
    {
        switch (inkType)
        {
            case InkType.RED:
                attackTypeImage.sprite = attackTypeImages[0];
                break;
            case InkType.GREEN:
                attackTypeImage.sprite = attackTypeImages[1];
                break;
            case InkType.BLUE:
                attackTypeImage.sprite = attackTypeImages[2];
                break;
        }
    }
    public IEnumerator AttackDelayCoroutine()
    {
        isAbleAttack = false;
        
        yield return attackDelay;

        isAbleAttack = true;
    }
    public void Attack()
    {
        if (!isAbleAttack || playerDashControllerScr.IsDashing || playerInkMagicControllerScr.IsUsingInkMagic || playerSkillControllerScr.IsUsingSkill) return;

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
        attackEnemy = utilsManager.FindMinDistanceObject(playerScr.Tr.position, playerScr.AttackRange+0.1f, 1 << 6);
    }

    // 공격 오브젝트(투명 막대기)를 부채꼴 모양으로 움직이며 닿는 모든 적들에게 데미지를 입힌다.
    public IEnumerator SweepArkAttack(float startDegree, float degreeAmount)
    {
        attackObj.SetActive(true);

        float attackTime = 0;
        float currDegree = startDegree;
        float targetDegree = startDegree + degreeAmount;

        attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + startDegree, 0);
        while (attackTime <= currAnimationLength - 0.2f)
        {
            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / (currAnimationLength-0.1f));

            attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + currDegree, 0);
            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, playerScr.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        attackObj.SetActive(false);
        yield break;
    }
}
