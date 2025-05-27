using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSkill : Skill
{
    protected float explosionRange;
    public float ExplosionRange { get; set; }

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
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
                    enemyComponent.CurHp -= skillBasicDamage.Value;
                }
            }
        }

        // TODO
        // Æø¹ß ÀÌÆåÆ® Ã³¸®
        Destroy(this.gameObject);
    }
}
