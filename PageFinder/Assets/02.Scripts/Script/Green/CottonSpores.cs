using UnityEngine;

public class CottonSpores : SkillScript, IChargableScript
{
    public CottonSpores()
    {
        scriptBehaviour = new SkillBehaviour();
        if (scriptBehaviour is SkillBehaviour skillBehaviour)
        {
            skillBehaviour.ChangeSkill("SkillBulletFan");
        }
    }

    public void ChargeBehaviour()
    {
        IChargeBehaviour chargeScriptBehaviour = scriptBehaviour as IChargeBehaviour;
        if (chargeScriptBehaviour != null)
        {
            chargeScriptBehaviour.ChargingBehaviour();
        }
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
                if (scriptBehaviour is SkillBehaviour skillBehaviour)
                {
                    skillBehaviour.ChangeSkill("InkSkillEvolved");
                }
                break;
            case 3:
                break;
        }
    }
}
