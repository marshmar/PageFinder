using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IType
{
    enum TYPE
    {
        RED,
        BLUE,
        GREEN,
        YELLOW,
        PURPLE,
        FIRE,
        GRASSLAND,
        DESERT,
        ELECTRIC
    }

    public TYPE CurrType { get; set; }

}
