using UnityEngine;
using System;

public class SkillScript : BaseScript
{
    protected GameObject skillObject;
    protected SkillData skillData;


    public float SkillCost { get => skillData.skillCost; }
    public SkillCastType SkillCastType {  get => skillData.skillCastType; } 
    public float SkillCoolTime {  get => skillData.skillCoolTime; }
    public SkillShapeType SkillShapeType {  get => skillData.skillShapeType; }  
    public float SkillDist { get => skillData.skillDist; }  
    public SkillData SkillData { get => skillData; }
    public float SkillRange { get => skillData.skillRange; }
    public GameObject SkillObject {  get => skillObject; }  

    public Stat SkillBasicDamage
    {
        get
        {
            if(scriptBehaviour is SkillBehaviour skillBehaviour)
            {
                return skillBehaviour.SkillBasicDamage;
            }
            return null;
        }
    }
    public SkillScript()
    {

    }

    public override void Upgrade(int rarity)
    {
        switch (rarity)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    public bool ChangeSkill(string skillName)
    {
        skillObject = SkillManager.Instance.GetSkillPrefab(skillName);
        if (skillObject == null)
        {
            return false;
        }

        // Todo: 현재 원본이 바뀌고 있음, 복사본 생성 코드로 변경 필요
        skillData = SkillManager.Instance.GetSkillData(skillName);
        if (skillData == null)
        {
            return false;
        }

        return true;
    }
}
    