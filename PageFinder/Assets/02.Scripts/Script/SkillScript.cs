using UnityEngine;

public class SkillScript : BaseScript
{
    public SkillScript()
    {
        scriptBehaviour = new SkillBehaviour();
        if (scriptBehaviour is SkillBehaviour skillBehaviour)
        {
            skillBehaviour.ChangeSkill("SkillBulletFan");
        }
    }
    public override void UpgrageScript(int rarity)
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

}

public class ChargableSkillScript : SkillScript
{
    public ChargableSkillScript()
    {

    }

    public virtual void ChargeBehaviour()
    {
        IChargeSkillBehaviour chargeScriptBehaviour = scriptBehaviour as IChargeSkillBehaviour;
        if (chargeScriptBehaviour != null)
        {
            chargeScriptBehaviour.ChargingBehaviour();
        }
    }
}
    