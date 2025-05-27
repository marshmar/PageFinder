using UnityEngine;

public class StrongStrike : Sticker
{
    public override void AttachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (target is BAScript baScript)
        {
            baScript.SetDamageMultiplier(stickerData.levelData[stickerData.rarity]);
            Debug.Log($"플레이어의 기본 공격 피해량 {stickerData.levelData[stickerData.rarity] * 100}%증가");
        }
    }

    public override void DetachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (target is BAScript baScript)
        {
            baScript.SetDamageMultiplier(0);
            Debug.Log($"플레이어의 기본 공격 피해량 증가 제거");
        }
    }
}
