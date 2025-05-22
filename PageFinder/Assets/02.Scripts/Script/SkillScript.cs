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
    public override void Upgrade(int rarity)
    {
        switch (rarity)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                if(scriptBehaviour is SkillBehaviour skillBehaviour)
                {
                    skillBehaviour.ChangeSkill("InkSkillEvolved");
                }
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
        IChargeBehaviour chargeScriptBehaviour = scriptBehaviour as IChargeBehaviour;
        if (chargeScriptBehaviour != null)
        {
            chargeScriptBehaviour.ChargingBehaviour();
        }
    }
}
    