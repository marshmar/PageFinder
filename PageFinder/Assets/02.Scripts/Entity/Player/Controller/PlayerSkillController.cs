using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillController : MonoBehaviour, IListener
{

    private string currSkillName;
    private SkillManager skillManager;
    private GameObject currSkillObject;
    private SkillData currSkillData;
    private PlayerAttackController playerAttackControllerScr;
    //private PlayerInkMagicController playerInkMagicControllerScr;
    // ��ų ��ȯ ����
    private Vector3 spawnVector;
    // ��ų ���������
    private bool isUsingSkill;
    // Ÿ���� ������
    private bool isOnTargeting;

    // ������ �� ��ü
    private Collider attackEnemy;

    private PlayerAnim playerAnim;
    private PlayerState playerState;
    private PlayerUtils playerUtils;
    private PlayerInkType playerInkType;
    private UtilsManager utilsManager;
    private PlayerInputAction input;
    private PlayerTarget playerTarget;
    public bool IsUsingSkill { get => isUsingSkill; set => isUsingSkill = value; }
    public bool IsOnTargeting { get => isOnTargeting; set => isOnTargeting = value; }
    public string CurrSkillName { get => currSkillName; set => currSkillName = value; }
    public SkillData CurrSkillData { get => currSkillData; set => currSkillData = value; }

    public bool fireWork = false;
    public float fireWorkValue = 0;

    private Vector3 skillDir;
    private bool isChargingSkill = false;
    private bool skillCanceled = false;
    public void Awake()
    {
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(this.gameObject, "PlayerInkType");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerTarget = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");

        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
        isUsingSkill = false;
    }
    // Start is called before the first frame update
    public void Start()
    {
        skillManager = SkillManager.Instance;
        utilsManager = UtilsManager.Instance;
        currSkillName = "SkillBulletFan";
        ChangeSkill(currSkillName);

        SetSkillAction();
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
    }

    private void SetSkillAction()
    {
        if (input is null)
        {
            Debug.LogError("PlayerInput ������Ʈ�� �������� �ʽ��ϴ�.");
            return;
        }

        if (input.SkillAction is null)
        {
            Debug.LogError("Skill Action�� �������� �ʽ��ϴ�.");
            return;
        }

        input.SkillAction.started += context => 
        {
            skillDir = Vector3.zero;
        };

        input.SkillAction.performed += context =>
        {
            if(CheckCanUseSkill())
                isChargingSkill = true;
        };

        input.SkillAction.canceled += context =>
        {
            if (!skillCanceled && CheckCanUseSkill())
            {
                if (!isChargingSkill)
                    InstantiateSkill();
                else
                    InstantiateSkill(/*playerUtils.Tr.position + */skillDir) ;
            }

            playerTarget.OffAllTargetObjects();
            skillCanceled = false;
            isChargingSkill = false;
        };

        if(input.CancelAction is null)
        {
            Debug.LogError("Cancel Action�� �������� �ʽ��ϴ�.");
            return;
        }

        input.CancelAction.started += context =>
        {
            playerTarget.OffAllTargetObjects();
            isChargingSkill = false;
            skillCanceled = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (isChargingSkill)
        {
            SetSkillDirection();
            if(currSkillData is FanSkillData fanSkillData){
                playerTarget.FanTargeting(skillDir, fanSkillData.skillRange, fanSkillData.fanDegree);
            }
        }

        if (isUsingSkill)
        {
            playerAnim.CheckAnimProgress(currSkillData.skillState, currSkillData.skillAnimEndTime, ref isUsingSkill);
        }

    }

    private void SetSkillDirection()
    {
        Vector2 screenDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(playerUtils.Tr.position);
        skillDir = new Vector3(screenDirection.x, 0f, screenDirection.y).normalized;
    }

    private bool CheckCanUseSkill()
    {
        return !isUsingSkill && playerState.CurInk >= currSkillData.skillCost && !playerAttackControllerScr.IsAttacking;
    }

    /// <summary>
    /// ���� ����� ������ ��ų�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <return>��ų ��ȯ ���� ����</return>
    public bool InstantiateSkill()
    {
        if (!isUsingSkill && playerState.CurInk >= currSkillData.skillCost && !playerAttackControllerScr.IsAttacking)
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                { 
                    switch (currSkillData.skillType)
                    {
                        case SkillTypes.FAN:
                            attackEnemy = utilsManager.FindMinDistanceObject(playerUtils.Tr.position, currSkillData.skillDist, 1 << 6);
                            if (!DebugUtils.CheckIsNullWithErrorLogging(attackEnemy, "������ �� ��ü�� �����ϴ�."))
                            {
                                isUsingSkill = true;


                                GameObject instantiatedSkill = Instantiate(currSkillObject, playerUtils.Tr.position, Quaternion.identity);
                                if (!DebugUtils.CheckIsNullWithErrorLogging(instantiatedSkill, this.gameObject))
                                {
                                    playerAnim.SetAnimationTrigger("TurningSkill");
                                    if (attackEnemy.transform.position == null) return false;
                                    spawnVector = attackEnemy.transform.position - playerUtils.Tr.position;
                                    playerUtils.TurnToDirection(spawnVector);
                                    Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                    if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                    {
                                        if(skill is BulletFanSkill bulletFanSkill)
                                        {
                                            if (fireWork) bulletFanSkill.bulletSpeed *= (1 + fireWorkValue);
                                        }
                                        skill.SkillInkType = playerInkType.SkillInkType;
                                        skill.ActiveSkill(spawnVector.normalized);
                                        playerState.CurInk -= currSkillData.skillCost;
                                        EventManager.Instance.PostNotification(EVENT_TYPE.Skill_Successly_Used, this);
                                        return true;
                                    }
                                }
                            }
                            break;
                        default:
                            spawnVector = new Vector3(playerUtils.Tr.position.x, playerUtils.Tr.position.y + 0.1f, playerUtils.Tr.position.z);
                            break;
                    }
                }
             }
        }
        return false;
    }

    // ������ ��ġ�� ��ų ��ȯ�ϴ� �Լ�
    public bool InstantiateSkill(Vector3 pos)
    {
        if (!isUsingSkill && playerState.CurInk >= currSkillData.skillCost && !playerAttackControllerScr.IsAttacking)
        {
            //rangedEntity.DisableCircleRenderer();
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                {
                    GameObject instantiatedSkill = Instantiate(currSkillObject, playerUtils.Tr.position, Quaternion.identity);
                    if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedSkill, this.gameObject))
                    {
                        switch (currSkillData.skillType)
                        {
                            case SkillTypes.FAN:
                                isUsingSkill = true;
                                playerUtils.TurnToDirection(pos);
                                playerAnim.SetAnimationTrigger("TurningSkill");
                                Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                {
                                    if (skill is BulletFanSkill bulletFanSkill)
                                    {
                                        if (fireWork) bulletFanSkill.bulletSpeed *= (1 + fireWorkValue);
                                    }
                                    skill.SkillInkType = playerInkType.SkillInkType;
                                    skill.ActiveSkill(pos.normalized);
                                    playerState.CurInk -= currSkillData.skillCost;
                                    EventManager.Instance.PostNotification(EVENT_TYPE.Skill_Successly_Used, this);
                                    return true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        
        return false;
    }

    /// <summary>
    /// ������ ��ų�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="skillName">������ ��ų �̸�</param>
    public bool ChangeSkill(string skillName)
    {
        if(!DebugUtils.CheckIsNullWithErrorLogging<SkillManager>(skillManager, this.gameObject))
        {
            this.currSkillObject = skillManager.GetSkillPrefab(skillName);
            if(DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                return false;
            }
            this.currSkillData = skillManager.GetSkillData(skillName);
            if (DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
            {
                return false;
            }
            this.currSkillData.skillInkType = playerInkType.SkillInkType;
        }
        return true;
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Joystick_Short_Released:
                if(sender.name == "Player_UI_OP_Skill")
                    InstantiateSkill();
                break;
            case EVENT_TYPE.Joystick_Long_Released:
                if(sender.name == "Player_UI_OP_Skill")
                {
                    Vector3 position = (Vector3)param;
                    InstantiateSkill(position);
                }
                break;
        }
    }

    public IEnumerator FireWork()
    {
        playerState.CurAttackSpeed += 0.1f;
        yield return new WaitForSeconds(3.0f);
        playerState.CurAttackSpeed -= 0.1f;
    }
}
