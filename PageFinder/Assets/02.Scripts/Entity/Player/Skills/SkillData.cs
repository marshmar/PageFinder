using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillData : ScriptableObject
{
    public SkillTypes skillType;
    public float skillCoolTime;
    public float skillBasicDamage;
    public float skillDuration;
    public float skillRange;
    public float skillDist;
    public string skillState;
    public float skillAnimEndTime;
}
