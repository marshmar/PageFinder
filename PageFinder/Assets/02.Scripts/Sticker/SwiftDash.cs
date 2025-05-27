using UnityEngine;

public class SwiftDash : Sticker
{
    public override void AttachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if(target is DashScript dashScript)
        {
            float decreseVal = stickerData.levelData[stickerData.rarity];
            dashScript.DashDuration.RemoveAllFromSource(this);
            dashScript.DashDuration.AddModifier(new StatModifier(-decreseVal, StatModifierType.PercentMultiplier, this));
            Debug.Log($"신속 질주 - 대쉬 속도 {decreseVal * 100}% 증가");
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
            dashScript.DashDuration.RemoveAllFromSource(this);
            Debug.Log($"신속 질주 - 대쉬 속도 증가효과 제거");
        }
    }
}
