using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnstoppableTouch : Skill
{
    private float rotateSpeed;

    // Start is called before the first frame update
    public override void Start()
    {
        rotateSpeed = 1440.0f;
        skillBasicDamage = 30.0f;
        skillCoolTime = 3.0f;
        base.Start();
        Destroy(tr.transform.gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        tr.RotateAround(PlayerObj.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("ENEMY"))
        {
            coll.gameObject.GetComponent<Enemy>().HP -= skillBasicDamage;
        }
    }
}
