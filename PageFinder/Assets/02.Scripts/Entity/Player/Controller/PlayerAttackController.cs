using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttackController : MonoBehaviour, IListener
{
    #region Variable
    // 공격할 적 객체
    private Collider attackEnemy;

    private int comboCount;
    private bool isAttacking;
    private bool isAbleAttack = true;
    private float currAnimationLength;
    private WaitForSeconds attackDelay;
    private float attackDelayValue = 2.0f;
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
    private PlayerInkType playerInkType;
    private PlayerInputAction input;
    private Coroutine attackDelayCoroutine;
    #endregion


    [Header("Effects")]
    [SerializeField] private GameObject[] baEffectRed;
    [SerializeField] private GameObject[] baEffectGreen;
    [SerializeField] private GameObject[] baEffectBlue;

    [SerializeField] private float dis = 0.5f;
    public bool IsAttacking { get => isAttacking; set { 
            isAttacking = value;

            if (!isAttacking)
            {
                attackDelayCoroutine = StartCoroutine(AttackDelayCoroutine());
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
        playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(this.gameObject, "PlayerInkType");

        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
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

        comboCount = 0;
        attackObj.SetActive(false);

        SetAttackAction();

        EventManager.Instance.AddListener(EVENT_TYPE.UI_Changed, this);
    }

    private void SetAttackAction()
    {
        if (input is null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (input.AttackAction is null)
        {
            Debug.LogError("Attack Action이 존재하지 않습니다.");
            return;
        }

        input.AttackAction.canceled += context =>
        {
            Attack();
        };
    }

    public IEnumerator AttackDelayCoroutine()
    {
        if (!isAbleAttack) yield break;

        isAbleAttack = false;

        yield return attackDelay;

        isAbleAttack = true;
    }

    public void SetAttckSpeed(float curAttackSpeed)
    {
        float attackDelayVal = attackDelayValue * (1 - curAttackSpeed);
        attackDelay = new WaitForSeconds(attackDelayVal);
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
            IsAttacking = true;
            playerAnim.SetAnimationTrigger("Attack");
            //targetObject.SetActive(false);
        }
    }

    // 공격 콤보에 따라 다른 크기의 각도로 공격을 하는 함수
    public void SweepArkAttackEachComboStep()
    {
        switch (ComboCount)
        {
            case 0:
                //AudioManager.Instance.Play(SoundPath.attack1SfxPath);
                StartCoroutine(SweepArkAttack(-45.0f, 90.0f));
                break;
            case 1:
                //AudioManager.Instance.Play(SoundPath.attack2SfxPath);
                StartCoroutine(SweepArkAttack(45.0f, -90.0f));
                break;
            case 2:
                //AudioManager.Instance.Play(SoundPath.attack3SfxPath);
                StartCoroutine(SweepArkAttack(-70.0f, 140.0f));
                break;
            default:
                break;
        }
        GameObject attackEffect = CreateEffectByType(ComboCount);
        attackEffect.transform.position = this.gameObject.transform.position - (dis * playerUtils.ModelTr.forward);
        attackEffect.transform.rotation = Quaternion.Euler(attackEffect.transform.rotation.eulerAngles.x, playerUtils.ModelTr.eulerAngles.y, 180f);
        Destroy(attackEffect, currAnimationLength - 0.2f);
    }
    
    public void SetAttackEnemy()
    {
        int targetLayer = (1 << 6) + (1 << 11); // Enemy + Interactive Object

#if UNITY_STANDALONE
        // PC 플랫폼일 경우 마우스 포지션에 위치한 적 먼저 공격
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(cameraRay, out hit, Mathf.Infinity, targetLayer))
        {
            Collider primaryAttackEnemy = hit.collider;
            if(Vector3.Distance(playerUtils.Tr.position, primaryAttackEnemy.transform.position) <= playerState.CurAttackRange)
            {
                attackEnemy = primaryAttackEnemy;
                return;
            }
            
        }
#endif
        // 기존에 공격하던 적 우선 공격
        if(attackEnemy is not null)
        {
            if(attackEnemy.ToString() != "null" && attackEnemy.gameObject.activeSelf)
            {
                if (Vector3.Distance(attackEnemy.transform.position, playerUtils.transform.position) <= playerState.CurAttackRange)
                {
                    return;
                }
            }
        }

        attackEnemy = Utils.FindMinDistanceObject(playerUtils.Tr.position, playerState.CurAttackRange, targetLayer);
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

    private GameObject CreateEffectByType(int comboCouunt)
    {
        GameObject attackEffect = null;
        switch (playerInkType.BasicAttackInkType)
        {
            case InkType.RED:
                attackEffect = Instantiate(baEffectRed[comboCouunt], this.transform);
                break;
            case InkType.GREEN:
                attackEffect = Instantiate(baEffectGreen[comboCouunt], this.transform);
                break;
            case InkType.BLUE:
                attackEffect = Instantiate(baEffectBlue[comboCouunt], this.transform);
                break;

        }

        return attackEffect;
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.UI_Changed:
                var uiChanged = (UIType)Param;
                CheckCanAttack(uiChanged);
                break;
        }
    }

    private void CheckCanAttack(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.Battle:
            case UIType.PageMap:
            case UIType.RiddlePlay:
                isAbleAttack = true;
                break;
            default:
                if(attackDelayCoroutine is not null)
                    StopCoroutine(attackDelayCoroutine);
                isAbleAttack = false;
                break;
        }
    }
}
