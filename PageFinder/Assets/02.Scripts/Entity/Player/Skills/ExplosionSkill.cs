using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSkill : Skill
{
    protected float explosionDamage;
    public float ExplosionDamage { get; set; }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public virtual void Explosion()
    {
        Collider[] enemies = Physics.OverlapSphere(tr.position, skillRange);
        if (enemies != null)
        {
            foreach (Collider enemy in enemies)
            {
                if (enemy.TryGetComponent<Enemy>(out Enemy enemyComponent))
                {
                    enemyComponent.HP -= skillBasicDamage;
                }
            }
        }
        // TODO
        // Æø¹ß ÀÌÆåÆ® Ã³¸®
        Destroy(this.gameObject);
    }
}
