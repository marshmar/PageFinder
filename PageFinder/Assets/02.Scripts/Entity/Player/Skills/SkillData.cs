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
}
