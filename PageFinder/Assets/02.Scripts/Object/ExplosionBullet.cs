using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBullet : Bullet
{
    public float explosionRange;
    private PlayerState playerState;
    private PlayerScriptController playerScriptController;
    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(playerObj, "PlayerScriptController");
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (bulletType == BulletType.PLAYER)
        {
            if (other.CompareTag("ENEMY") || other.CompareTag("MAP"))
            {
                GenerateInkMark(other.ClosestPoint(tr.position));
                Explosion(1 << 6);
                if(BulletInkType == InkType.GREEN)
                {
                    if(playerState is not null)
                        EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Player, this, 
                            new System.Tuple<float, float>(playerState.MaxHp * playerScriptController.PlayerSkillScriptData.percentages[playerScriptController.PlayerSkillScriptData.level], 2f));
                }   
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
                }
            }
        }
        // TODO
        // ���� ����Ʈ ó��
        Destroy(this.gameObject);
    }
}
