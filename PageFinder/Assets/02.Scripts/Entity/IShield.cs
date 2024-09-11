using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShield
{
    public float MaxShield
    {
        get
        {
            return MaxShield;
        }
        set
        {
            MaxShield = value;
        }
    }

    public float CurrShield
    {
        get
        {
            return CurrShield;
        }
        set
        {
            CurrShield = value;
        }
    }
}
