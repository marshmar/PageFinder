using UnityEngine;

public class DashScript : BaseScript
{
    public DashScript()
    {
        scriptBehaviour = new DashBehaviour();
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
}

public class ChargableDashScriipt : DashScript
{
    public virtual void ChargeBehaviour()
    {
        IChargeBehaviour chargeScriptBehaviour = scriptBehaviour as IChargeBehaviour;
        if (chargeScriptBehaviour != null)
        {
            chargeScriptBehaviour.ChargingBehaviour();
        }
    }
}
