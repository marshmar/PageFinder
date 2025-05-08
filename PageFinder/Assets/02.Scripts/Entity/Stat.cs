using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class Stat
{
    protected float baseValue;
    [SerializeField] protected List<StatModifier> flatPermanent = new List<StatModifier>();
    [SerializeField] protected List<StatModifier> percentMultiplier = new List<StatModifier>();
    [SerializeField] protected List<StatModifier> percentAddTemporary = new List<StatModifier>();

    public float BaseValue { get => baseValue;}

    public event Action OnModified;

    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
    }

    public void AddModifier(StatModifier mod)
    {
        switch (mod.Type)
        {
            case StatModifierType.FlatPermanent:
                flatPermanent.Add(mod);
                break;
            case StatModifierType.PercentMultiplier:
                percentMultiplier.Add(mod);
                break;
            case StatModifierType.PercentAddTemporary:
                percentAddTemporary.Add(mod);
                break;
        }

        OnModified?.Invoke();
    }

    public void RemoveAllFromSource(object source)
    {
        flatPermanent.RemoveAll(m => m.Source == source);
        percentMultiplier.RemoveAll(m => m.Source == source);
        percentAddTemporary.RemoveAll(m => m.Source == source);

        OnModified?.Invoke();
    }

    // FinalStat = (BaseStat + PermanentBuff) ¡¿ (PermanentMultiplier) x (1 - PermanentDebuff) ¡¿ (1 + TemporaryBuff - TemporaryDebuff)
    public virtual float Value
    {
        get
        {
            float flatSum = flatPermanent.Sum(m => m.Value);
            float multSum = percentMultiplier.Sum(m => m.Value);
            float tempSum = percentAddTemporary.Sum(m => m.Value);

            float basePlus = baseValue + flatSum;
            float afterMult = basePlus * (1 + multSum);
            return afterMult * (1 + tempSum);
        }
    }
}
