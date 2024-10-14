using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillTypes
{
    BASICATTACK, STROKE, PAINT, FAN
}

public class Skill : MonoBehaviour, IType
{
    protected GameObject playerObj;
    protected Transform tr;

    protected SkillTypes skillType;
    protected float skillCoolTime;
    protected float skillBasicDamage;
    protected float skillDuration;
    protected float skillRange;
    protected float skillDist;
    protected string skillState;
    [Range(0, 1.0f)]
    protected float skillAnimEndTime;
    protected IType.TYPE currType = IType.TYPE.PURPLE;


    public SkillData skillData;
    public SkillTypes SkillType { get; set; }
    public float SkillCoolTime { get; set; }
    public float SkillBasicDamage { get; set; }
    public float SkillDuration { get; set; }
    public float SkillRange { get; set; }
    public float SkillDist { get; set; }
    public IType.TYPE CurrType { get; set; }
    protected string SkillState { get => skillState; set => skillState = value; }
    protected float SkillAnimEndTime { get => skillAnimEndTime; set => skillAnimEndTime = value; }

    // Start is called before the first frame update
    public virtual void Start()
    {
        Hashing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void SetSkillData()
    {
        if (skillData == null)
        {
            Debug.LogError("스킬 데이터 없음");
            return;
        }

        skillType = skillData.skillType;
        skillCoolTime = skillData.skillCoolTime;
        skillBasicDamage = skillData.skillBasicDamage;
        skillDuration = skillData.skillDuration;
        skillRange = skillData.skillRange;
        skillDist = skillData.skillDist;
        skillState = skillData.skillState;
        skillAnimEndTime = skillData.skillAnimEndTime;
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
