using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SkillType
{
    STROKE, PAINT
}
public class Skill : MonoBehaviour
{
    protected Transform tr;
    protected float skillCoolTime;
    protected float skillBasicDamage;


    public float SkillCoolTime { get; set; }
    public float SkillBasicDamage { get; set; }
    // Start is called before the first frame update
    public virtual void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
