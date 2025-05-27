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
            Debug.Log($"��� ��ũ �Ҹ� {decreaseVal * 100}% ����");
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
            Debug.Log("���� �칰 ȿ�� ����");
        }

    }
}
