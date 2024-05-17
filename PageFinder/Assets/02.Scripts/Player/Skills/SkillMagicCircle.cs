using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMagicCircle : Skill
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Destroy(this, 2.0f);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("ENEMY")){
            coll.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
