using UnityEngine;

public class PerceivedTemperature : Sticker
{
    private Player player;

    public PerceivedTemperature()
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
        player.State.CurMoveSpeed.RemoveAllFromSource(this);
        player.State.CurMoveSpeed.AddModifier(new StatModifier(increaseVal, StatModifierType.PercentMultiplier, this));
        Debug.Log($"ü�� �µ� �߰� - �̵� �ӵ� {increaseVal * 100}% ����");
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

        player.State.CurMoveSpeed.RemoveAllFromSource(this);
        Debug.Log($"ü�� �µ� ȿ�� ����");
    }
}
