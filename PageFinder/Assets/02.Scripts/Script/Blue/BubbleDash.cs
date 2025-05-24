using UnityEngine;

public class BubbleDash : DashScript, IChargableScript
{
    public void ChargeBehaviour()
    {
        IChargeBehaviour chargeScriptBehaviour = scriptBehaviour as IChargeBehaviour;
        if (chargeScriptBehaviour != null)
        {
            chargeScriptBehaviour.ChargingBehaviour();
        }
    }
}
