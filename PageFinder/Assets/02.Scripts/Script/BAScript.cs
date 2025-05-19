using UnityEngine;

public class BAScript : BaseScript
{
    public BAScript()
    {
        scriptBehaviour = new BasicAttackBehaviour();
    }

    public override void UpgrageScript(int rarityPlus)
    {
        if (IsMaxRarity()) return;
        scriptData.rarity += rarityPlus;

        switch (GetRarity())
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                Debug.Log("Mechanism Chagne");
                break;
            case 3:
                break;
        }
    }
}
