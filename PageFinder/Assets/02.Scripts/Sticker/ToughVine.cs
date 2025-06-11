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

        Debug.Log("�＾ ���� ȿ�� - �ǵ� ���� ����");
    }

    public override void DetachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }
        target.AfterEffect -= GenerateShield;
        Debug.Log("�＾ ���� ȿ�� - �ǵ� ���� ����");

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
