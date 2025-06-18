using UnityEngine;

[System.Serializable]
public class BAScript : BaseScript
{
    public BAScript()
    {
    }

    public override void Upgrade(int rarity)
    {
        switch (rarity)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    public void SetDamageMultiplier(float amount)
    {
        if(scriptBehaviour is BasicAttackBehaviour basicAttackBehaviour)
        {
            basicAttackBehaviour.SetExtraDamageMultiplier(amount);
        }
    }
}
