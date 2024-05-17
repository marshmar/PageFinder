using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnstoppableTouch : Skill
{
    // Start is called before the first frame update
    public override void Start()
    {
        skillBasicDamage = 10.0f;
        skillCoolTime = 3.0f;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
