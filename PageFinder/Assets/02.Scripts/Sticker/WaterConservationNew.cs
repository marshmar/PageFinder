using UnityEngine;

public class WaterConservationNew : Sticker
{
    private Player player;
    public WaterConservationNew()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
    }
    public override void AttachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player Component is null");
            return;
        }

        float increaseVal = stickerData.levelData[stickerData.rarity];
        player.State.MaxHp.RemoveAllFromSource(this);
        player.State.MaxInk.AddModifier(new StatModifier(increaseVal, StatModifierType.PercentMultiplier, this));
        Debug.Log($"�� ���� �߰� - �ִ� ��ũ {increaseVal * 100}% ����");
    }

    public override void DetachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player Component is null");
            return;
        }

        player.State.MaxHp.RemoveAllFromSource(this);
        Debug.Log($"�� ���� ȿ�� ����");
    }
}
