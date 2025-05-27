using UnityEngine;

public class ForceOfThePlants : Sticker
{
    private Player player;

    public ForceOfThePlants()
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

        if (player ==null)
        {
            Debug.LogError("Player Component is null");
            return;
        }

        float increaseVal = stickerData.levelData[stickerData.rarity];
        player.State.MaxHp.RemoveAllFromSource(this);
        player.State.MaxHp.AddModifier(new StatModifier(increaseVal, StatModifierType.PercentMultiplier,this));
        Debug.Log($"초목의 기운 추가 - 최대 체력 {increaseVal * 100}% 증가");
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
        Debug.Log($"초목의 기운 효과 제거");
    }
}
