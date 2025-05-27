using UnityEngine;

public class DeepWellNew : Sticker
{
    
    public override void AttachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (target is DashScript dashScript)
        {
            float decreaseVal = stickerData.levelData[stickerData.rarity];
            dashScript.DashCost.RemoveAllFromSource(this);
            dashScript.DashCost.AddModifier(new StatModifier(-decreaseVal, StatModifierType.PercentAddTemporary, this));
            Debug.Log($"대시 잉크 소모량 {decreaseVal * 100}% 감소");
        }
    }

    public override void DetachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (target is DashScript dashScript)
        {
            dashScript.DashCost.RemoveAllFromSource(this);
            Debug.Log("깊은 우물 효과 제거");
        }

    }
}
