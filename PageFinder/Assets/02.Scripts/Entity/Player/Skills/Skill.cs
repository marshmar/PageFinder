using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillCastType
{
    DirectionBased, PositionBased, TargetBased
}

public enum SkillShapeType
{
    Line, Fan, Circle
}

public class Skill : MonoBehaviour
{
    protected GameObject playerObj;
    protected Transform tr;

    protected SkillCastType skillCastType;
    protected SkillShapeType skillShapeType;
    protected float skillCoolTime;
    protected Stat skillBasicDamage;
    protected float skillDuration;
    protected float skillRange;
    protected float skillDist;
    protected string skillState;
    protected float skillCost;
    [Range(0, 1.0f)]
    protected float skillAnimEndTime;
    protected InkType skillInkType;

    public SkillData skillData;
    public SkillCastType SkillCastType { get; set; }
    public SkillShapeType SkillShapeType { get; set; }
    public float SkillCoolTime { get; set; }
    public Stat SkillBasicDamage { get => skillBasicDamage; set => skillBasicDamage = value; }
    public float SkillDuration { get; set; }
    public float SkillRange { get; set; }
    public float SkillDist { get; set; }
    public string SkillState { get => skillState; set => skillState = value; }
    public float SkillAnimEndTime { get => skillAnimEndTime; set => skillAnimEndTime = value; }
    public InkType SkillInkType { get => skillInkType; set => skillInkType = value; }
    public float SkillCost { get => skillCost; set => skillCost = value; }

    // Start is called before the first frame update
    public virtual void Awake()
    {
        Hashing();
    }


    protected virtual void SetSkillData()
    {
        if (skillData == null)
        {
            Debug.LogError("스킬 데이터 없음");
            return;
        }

        skillCastType = skillData.skillCastType;
        skillShapeType = skillData.skillShapeType;
        skillCoolTime = skillData.skillCoolTime;
        skillBasicDamage = new Stat(skillData.skillBasicDamage);
        skillDuration = skillData.skillDuration;
        skillRange = skillData.skillRange;
        skillDist = skillData.skillDist;
        skillState = skillData.skillState;
        skillAnimEndTime = skillData.skillAnimEndTime;
        skillCost = skillData.skillCost;
    }


    public void Hashing()
    {
        playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        tr = GetComponent<Transform>();
    }

    protected bool CheckDistanceToDestroy(Vector3 originPos, Vector3 currPos)
    {
        float distance = Vector3.Distance(originPos, tr.position);
        return distance >= skillDist ? true : false;
    }

    public virtual void ActiveSkill() { }
    public virtual void ActiveSkill(Vector3 direction) { }
}
