using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnstoppableTouch : Skill
{
    private float rotateSpeed;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        SetSkillData();
        tr.position = playerObj.transform.position + new Vector3(0, 0, 3.0f);
        Destroy(this.gameObject, skillDuration);
        rotateSpeed = 1440.0f;
    }

    // Update is called once per frame
    void Update()
    {
        tr.RotateAround(playerObj.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("ENEMY"))
        {
            coll.gameObject.GetComponent<Enemy>().HP -= skillBasicDamage;
        }
    }
}
