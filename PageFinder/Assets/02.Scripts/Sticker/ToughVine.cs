using UnityEngine;

public class ToughVine : Sticker
{
    private Player player;
    public override void AttachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
        target.AfterEffect += GenerateShield;

        Debug.Log("억센 덩쿨 효과 - 실드 생성 연결");
    }

    public override void DetachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }
        target.AfterEffect -= GenerateShield;
        Debug.Log("억센 덩쿨 효과 - 실드 생성 해제");

    }

    private void GenerateShield()
    {
        if(player == null)
        {
            Debug.LogError("Player Component is null");
            return;
        }

        float shieldAmount = player.State.MaxHp.Value * stickerData.levelData[stickerData.rarity];
        float shieldDuration = 3f;
        InkType shieldInkType = target.GetInkType();

        EventManager.Instance.PostNotification( EVENT_TYPE.Generate_Shield_Player, null,
            new System.Tuple<float, float, InkType>(shieldAmount, shieldDuration, shieldInkType));
    }
}
