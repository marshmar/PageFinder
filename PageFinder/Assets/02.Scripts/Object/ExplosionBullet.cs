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
                // 13: Ground Layer;
                int targetLayer = LayerMask.GetMask("GROUND");
                Ray groundRay = new Ray(other.transform.position, Vector3.down);
                RaycastHit hit;
                Vector3 markSpawnPos = other.transform.position;
                if (Physics.Raycast(groundRay, out hit, Mathf.Infinity, targetLayer))
                {
                    markSpawnPos = hit.point + new Vector3(0f, 0.1f, 0f);
                }

                GenerateInkMark(markSpawnPos);
                Explosion(1 << 6);
/*                if(BulletInkType == InkType.GREEN)
                {
                    if(playerState is not null)
                        EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Player, this, 
                            new System.Tuple<float, float>(playerState.MaxHp.Value * playerScriptController.PlayerSkillScriptData.percentages[playerScriptController.PlayerSkillScriptData.level], 2f));
                } */  
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
                EnemyAction enemyAct = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy.transform, "EnemyAction") as EnemyAction;
                if(!DebugUtils.CheckIsNullWithErrorLogging<EnemyAction>(enemyAct, this.gameObject))
                {
                    enemyAct.Hit(BulletInkType, damage);
                }
            }
        }
        // TODO
        // Æø¹ß ÀÌÆåÆ® Ã³¸®
        Destroy(this.gameObject);
    }
}
