using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBullet : Bullet
{
    public float explosionRange;

    public override void OnTriggerEnter(Collider other)
    {
        if (bulletType == BulletType.PLAYER)
        {
            if (other.CompareTag("ENEMY") || other.CompareTag("MAP"))
            {
                GameObject instantiatedMark = GenerateInkMark(other.ClosestPoint(tr.position));
                if(!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedMark, this.gameObject))
                {
                    Transform tr = DebugUtils.GetComponentWithErrorLogging<Transform>(instantiatedMark, "Transform");
                    if(!DebugUtils.CheckIsNullWithErrorLogging<Transform>(tr, this.gameObject))
                    {
                        tr.localScale = new Vector3(3.0f, 3.0f, 0f);
                    }
                }
                Explosion(1 << 6);
            }
        }
        else if (bulletType == BulletType.ENEMY)
        {

        }
    }

    public void Explosion(LayerMask layer)
    {
        Collider[] enemies = Physics.OverlapSphere(tr.position, explosionRange, layer);
        if (enemies != null)
        {
            foreach (Collider enemy in enemies)
            {
                Enemy enemyScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(enemy.transform, "Enemy");
                if(!DebugUtils.CheckIsNullWithErrorLogging<Enemy>(enemyScr, this.gameObject))
                {
                    enemyScr.HP -= damage;
                    Debug.Log("Explosion Damagae");
                }
            }
        }
        // TODO
        // Æø¹ß ÀÌÆåÆ® Ã³¸®
        Destroy(this.gameObject);
    }
}
