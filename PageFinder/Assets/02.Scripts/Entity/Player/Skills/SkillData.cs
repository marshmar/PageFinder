using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillData : ScriptableObject
{
    public InkType skillInkType;
    public SkillCastType skillCastType;
    public SkillShapeType skillShapeType;
    public float skillCoolTime;
    public float skillBasicDamage;
    public float skillDuration;
    public float skillRange;
    public float skillDist;
    public string skillState;
    public float skillAnimEndTime;
    public float skillCost;

    public void CopyData(SkillData source)
    {
        skillInkType = source.skillInkType;
        skillCastType = source.skillCastType;
        skillShapeType = source.skillShapeType; 
        skillCoolTime = source.skillCoolTime;
        skillBasicDamage = source.skillBasicDamage;
        skillDuration = source.skillDuration;
        skillRange = source.skillRange;
        skillDist = source.skillDist;
        skillState = source.skillState;
        skillAnimEndTime = source.skillAnimEndTime;
        skillCost = source.skillCost;
    }
}
