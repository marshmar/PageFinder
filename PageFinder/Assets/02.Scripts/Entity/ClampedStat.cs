using UnityEngine;
using System.Linq;

public class ClampedStat : Stat
{
    private float minValue;
    private float maxValue;

    public ClampedStat(float baseValue, float minValue, float maxValue) : base(baseValue)
    {
        this.baseValue = baseValue;
        this.minValue = minValue;
        this.maxValue = maxValue;  
    }

    public override float Value
    {
        get
        {
            float flatSum = flatPermanent.Sum(m => m.Value);
            float multSum = percentMultiplier.Sum(m => m.Value);
            float tempSum = percentAddTemporary.Sum(m => m.Value);

            float basePlus = baseValue + flatSum;
            float afterMult = basePlus * (1 + multSum);

            float result = afterMult * (1 + tempSum);
            float clampedResult = Mathf.Clamp(afterMult, minValue, maxValue);

            return clampedResult;
        }
    }
}
