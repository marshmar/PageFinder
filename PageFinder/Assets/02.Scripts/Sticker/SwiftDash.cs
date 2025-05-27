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
            Debug.Log($"�ż� ���� - �뽬 �ӵ� {decreseVal * 100}% ����");
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
            Debug.Log($"�ż� ���� - �뽬 �ӵ� ����ȿ�� ����");
        }
    }
}
