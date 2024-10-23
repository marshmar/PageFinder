using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stel_BA_1 : ExplosionSkill
{
    private Vector3 originPos;
    private Vector3 dir;
    public Vector3 Dir
    {
        get { return dir; }
        set { dir = value; }
    }
    private Transform enemyTransform;
    public Transform EnemyTransform { get => enemyTransform; set => enemyTransform = value; }

    private float skillSpeed;
    private float startYposOffset;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        SetSkillData();
        originPos = tr.position;

        skillSpeed = 6.0f;
        startYposOffset = 0.5f;
        tr.position = new Vector3(tr.position.x, tr.position.y + startYposOffset, tr.position.z);
    }

    private void Update()
    {
        dir = (enemyTransform.position - tr.position).normalized;

        // 적을 향해 이동
        tr.position += new Vector3(dir.x, 0, dir.z) * skillSpeed * Time.deltaTime;

        if (CheckDistanceToDestroy(originPos, tr.position))
            Explosion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Enemy>(out Enemy enemyComponent))
        {
            enemyComponent.HP -= skillBasicDamage;
            Explosion();
        }
    }

}
