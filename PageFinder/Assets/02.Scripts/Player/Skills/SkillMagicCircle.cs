using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMagicCircle : Skill
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        skillDuration = 2.0f;
        Destroy(tr.parent.gameObject, skillDuration);
    }
    
    public void Update()
    {

    }
    // Update is called once per frame
    void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("ENEMY")){
            coll.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            coll.gameObject.GetComponent<EnemyController>().Die();
        }
    }
}
