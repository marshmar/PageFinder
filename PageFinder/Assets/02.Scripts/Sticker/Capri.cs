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
            Debug.Log($"ī���� - ��ų ���ط� {increaseVal * 100}% ����");
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
            Debug.Log($"ī���� - ��ų ���ط� ����ȿ�� ����");
        }
    }
}
