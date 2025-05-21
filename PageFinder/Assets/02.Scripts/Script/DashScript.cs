using UnityEngine;

public class DashScript : BaseScript
{
    public DashScript()
    {
        scriptBehaviour = new DashBehaviour();
    }
    public override void UpgrageScript(int rarity)
    {
        
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
