using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMagicCircle : Skill
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        SetSkillData();
        tr.localScale = new Vector3(skillRange, 0f, skillRange);
        Destroy(this.gameObject, skillDuration);
    }
    
    public void Update()
    {

    }

    // Update is called once per frame
    void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("ENEMY")){
            coll.gameObject.GetComponent<Enemy>().HP -= skillBasicDamage;
        }
    }
}
