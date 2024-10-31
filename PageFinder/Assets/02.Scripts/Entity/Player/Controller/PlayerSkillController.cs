using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{

    private string currSkillName;
    private SkillManager skillManager;
    private GameObject currSkillObject;
    private SkillData currSkillData;
    public InkType skillInkType;
    // 스킬 소환 벡터
    private Vector3 spawnVector;
    // 스킬 사용중인지
    private bool isUsingSkill;
    // 타겟팅 중인지
    private bool isOnTargeting;

    // 공격할 적 객체
    private Collider attackEnemy;

    private Player playerScr;
    private UtilsManager utilsManager;

    public bool IsUsingSkill { get => isUsingSkill; set => isUsingSkill = value; }
    public bool IsOnTargeting { get => isOnTargeting; set => isOnTargeting = value; }
    public string CurrSkillName { get => currSkillName; set => currSkillName = value; }
    public SkillData CurrSkillData { get => currSkillData; set => currSkillData = value; }
    public InkType SkillInkType { get => skillInkType; set => skillInkType = value; }

    public void Awake()
    {
        skillInkType = InkType.BLUE;
        isUsingSkill = false;
    }
    // Start is called before the first frame update
    public void Start()
    {
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        skillManager = SkillManager.Instance;
        utilsManager = UtilsManager.Instance;
        currSkillName = "SkillBulletFan";
        ChangeSkill(currSkillName);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsingSkill)
        {
            playerScr.CheckAnimProgress(currSkillData.skillState, currSkillData.skillAnimEndTime, ref isUsingSkill);
        }
    }

    /// <summary>
    /// 가장 가까운 적에게 스킬을 소환하는 함수
    /// </summary>
    /// <return>스킬 소환 성공 여부</return>
    public bool InstantiateSkill()
    {
        if (!isUsingSkill && playerScr.CurrInk >= currSkillData.skillCost)
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                { 
                    switch (currSkillData.skillType)
                    {
                        case SkillTypes.FAN:
                            attackEnemy = utilsManager.FindMinDistanceObject(playerScr.Tr.position, currSkillData.skillDist, 1 << 6);
                            if (!DebugUtils.CheckIsNullWithErrorLogging(attackEnemy, "공격할 적 객체가 없습니다."))
                            {
                                GameObject instantiatedSkill = Instantiate(currSkillObject, playerScr.Tr.position, Quaternion.identity);
                                if (!DebugUtils.CheckIsNullWithErrorLogging(instantiatedSkill, this.gameObject))
                                {
                                    playerScr.Anim.SetTrigger("TurningSkill");
                                    spawnVector = attackEnemy.transform.position - playerScr.Tr.position;
                                    Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                    if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                    {
                                        skill.SkillInkType = this.skillInkType;
                                        skill.ActiveSkill(spawnVector.normalized);
                                        playerScr.CurrInk -= skill.SkillCost;
                                        playerScr.RecoverInk();
                                        isUsingSkill = true;
                                        return true;
                                    }
                                }
                            }
                            break;
                        default:
                            spawnVector = new Vector3(playerScr.Tr.position.x, playerScr.Tr.position.y + 0.1f, playerScr.Tr.position.z);
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
        if (!isUsingSkill && playerScr.CurrInk >= currSkillData.skillCost)
        {
            //rangedEntity.DisableCircleRenderer();
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                {
                    GameObject instantiatedSkill = Instantiate(currSkillObject, playerScr.Tr.position, Quaternion.identity);
                    if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedSkill, this.gameObject))
                    {
                        switch (currSkillData.skillType)
                        {
                            case SkillTypes.FAN:
                                playerScr.TurnToDirection(pos);
                                playerScr.Anim.SetTrigger("TurningSkill");
                                Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                {
                                    skill.SkillInkType = this.skillInkType;
                                    skill.ActiveSkill(pos.normalized);
                                    playerScr.CurrInk -= skill.SkillCost;
                                    playerScr.RecoverInk();
                                    isUsingSkill = true;
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
            this.currSkillData.skillInkType = this.skillInkType;
        }
        return true;
    }
}
