using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Skill : MonoBehaviour
{
    public enum SkillTypes
    {
        STROKE, PAINT
    }

    protected GameObject PlayerObj;
    protected Transform tr;
    protected SkillTypes skillType;
    protected float skillCoolTime;
    protected float skillBasicDamage;
    protected float skillDuration;

    public SkillTypes SkillType { get; set; }
    public float SkillCoolTime { get; set; }
    public float SkillBasicDamage { get; set; }
    public float SkillDuration { get; set; }
    // Start is called before the first frame update
    public virtual void Start()
    {
        PlayerObj = GameObject.FindGameObjectWithTag("PLAYER");
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
