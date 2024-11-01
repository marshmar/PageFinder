using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RallyPoints : MonoBehaviour
{
    bool[] canUse = { true,true,true,true,true};


    public void SetUseState(int index, bool value)
    {
        canUse[index] = value;
    }

    public bool CheckIfCanUseRallyPoint(int index)
    {
        return canUse[index];
    }
}
