using UnityEngine;

public class Fernand : Sticker
{
    private Player player;
    public Fernand()
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
        player.State.CurAtk.RemoveAllFromSource(this);
        player.State.CurAtk.AddModifier(new StatModifier(increaseVal, StatModifierType.PercentMultiplier, this));
        Debug.Log($"페르난드 추가 - 공격력 {increaseVal * 100}% 증가");
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

        player.State.CurAtk.RemoveAllFromSource(this);
        Debug.Log($"페르난드 효과 제거");
    }
}
