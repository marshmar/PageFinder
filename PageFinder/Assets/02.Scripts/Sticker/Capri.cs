using UnityEngine;

public class Capri : Sticker
{
    public override void AttachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (target is SkillScript skillScript)
        {
            float increaseVal = stickerData.levelData[stickerData.rarity];
            skillScript.SkillBasicDamage.RemoveAllFromSource(this);
            skillScript.SkillBasicDamage.AddModifier(new StatModifier(increaseVal, StatModifierType.PercentMultiplier, this));
            Debug.Log($"카프리 - 스킬 피해량 {increaseVal * 100}% 증가");
        }
    }

    public override void DetachStickerLogic()
    {
        if (target == null)
        {
            Debug.LogError("Sticker Logic target is null");
            return;
        }

        if (target is SkillScript skillScript)
        {
            skillScript.SkillBasicDamage.RemoveAllFromSource(this);
            Debug.Log($"카프리 - 스킬 피해량 증가효과 제거");
        }
    }
}
