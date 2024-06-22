using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillTypes
{
    NONE, STROKE, PAINT
}

public class Skill : MonoBehaviour
{

    protected GameObject playerObj;
    protected Transform tr;

    protected SkillTypes skillType;
    protected float skillCoolTime;
    protected float skillBasicDamage;
    protected float skillDuration;
    protected float skillRange;
    protected float skillDist;

    public SkillData skillData;
    public SkillTypes SkillType { get; set; }
    public float SkillCoolTime { get; set; }
    public float SkillBasicDamage { get; set; }
    public float SkillDuration { get; set; }
    public float SkillRange { get; set; }
    public float SkillDist { get; set; }

    // Start is called before the first frame update
    public virtual void Start()
    {
        Hashing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void SetSkillData()
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
    }
    /// <summary>
    /// 스킬 값 설정
    /// </summary>
    /// <param name="skillType">스킬 타입</param>
    /// <param name="skillCoolTime">스킬 쿨타임</param>
    /// <param name="skillBasicDamage">스킬 기본 데미지</param>
    /// <param name="skillDuration">스킬 지속 시간</param>
    /// <param name="skillRange">스킬 범위</param>
    /// <param name="skillDist">스킬 사거리</param>
    protected void SetSkillStatus(SkillTypes skillType, float skillCoolTime, float skillBasicDamage, 
        float skillDuration, float skillRange, float skillDist)
    {
        this.skillType = skillType;
        this.skillCoolTime = skillCoolTime;
        this.skillBasicDamage = skillBasicDamage;
        this.skillDuration = skillDuration;
        this.skillRange = skillRange;
        this.skillDist = skillDist;
    }

    public void Hashing()
    {
        playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        tr = GetComponent<Transform>();
    }
}
