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
            Debug.Log($"�÷��̾��� �⺻ ���� ���ط� {stickerData.levelData[stickerData.rarity] * 100}%����");
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
            Debug.Log($"�÷��̾��� �⺻ ���� ���ط� ���� ����");
        }
    }
}
