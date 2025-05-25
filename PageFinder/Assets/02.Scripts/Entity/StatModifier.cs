using UnityEngine;

public enum StatModifierType
{
    FlatPermanent,
    PercentMultiplier,
    PercentAddTemporary
    
}
public class StatModifier
{
    public float Value;
    public StatModifierType Type;
    public object Source;

    public StatModifier(float value, StatModifierType type, object source)
    {
        this.Value = value;
        this.Type = type;
        this.Source = source;
    }
}
