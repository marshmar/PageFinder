using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour, IListener
{

    private string currSkillName;
    private SkillManager skillManager;
    private GameObject currSkillObject;
    private SkillData currSkillData;
    private PlayerAttackController playerAttackControllerScr;
    //private PlayerInkMagicController playerInkMagicControllerScr;
    // 스킬 소환 벡터
    private Vector3 spawnVector;
    // 스킬 사용중인지
    private bool isUsingSkill;
    // 타겟팅 중인지
    private bool isOnTargeting;

    // 공격할 적 객체
    private Collider attackEnemy;

    private PlayerAnim playerAnim;
    private PlayerState playerState;
    private PlayerUtils playerUtils;
    private PlayerInkType playerInkType;
    private UtilsManager utilsManager;

    public bool IsUsingSkill { get => isUsingSkill; set => isUsingSkill = value; }
    public bool IsOnTargeting { get => isOnTargeting; set => isOnTargeting = value; }
    public string CurrSkillName { get => currSkillName; set => currSkillName = value; }
    public SkillData CurrSkillData { get => currSkillData; set => currSkillData = value; }


    public void Awake()
    {
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(this.gameObject, "PlayerInkType");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        isUsingSkill = false;
    }
    // Start is called before the first frame update
    public void Start()
    {
        skillManager = SkillManager.Instance;
        utilsManager = UtilsManager.Instance;
        currSkillName = "SkillBulletFan";
        ChangeSkill(currSkillName);

        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsingSkill)
        {
            playerAnim.CheckAnimProgress(currSkillData.skillState, currSkillData.skillAnimEndTime, ref isUsingSkill);
        }

    }

    /// <summary>
    /// 가장 가까운 적에게 스킬을 소환하는 함수
    /// </summary>
    /// <return>스킬 소환 성공 여부</return>
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
                            if (!DebugUtils.CheckIsNullWithErrorLogging(attackEnemy, "공격할 적 객체가 없습니다."))
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

    // 지정한 위치에 스킬 소환하는 함수
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
    /// 장착된 스킬을 변경하는 함수
    /// </summary>
    /// <param name="skillName">변경할 스킬 이름</param>
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
}
