using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatModifier
{
    public void AddDecorator();
    public float ModifyStat(float value);
}
