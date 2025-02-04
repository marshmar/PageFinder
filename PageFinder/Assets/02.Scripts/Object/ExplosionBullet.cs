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
                GenerateInkMark(other.ClosestPoint(tr.position));
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
                Entity entityScr = DebugUtils.GetComponentWithErrorLogging<Entity>(enemy.transform, "Entity");
                if(!DebugUtils.CheckIsNullWithErrorLogging<Entity>(entityScr, this.gameObject))
                {
                    entityScr.HP -= damage;
                    Debug.Log("Explosion Damagae");
                }
            }
        }
        // TODO
        // ���� ����Ʈ ó��
        Destroy(this.gameObject);
    }
}
