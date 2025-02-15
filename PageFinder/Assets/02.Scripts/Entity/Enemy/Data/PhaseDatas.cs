using System.Collections.Generic;
using UnityEngine;

public class PhaseDatas : MonoBehaviour
{
    public List<PhaseData> phaseDatas = new List<PhaseData>();


    private void SetPhaseDatas()
    {
        for(int i=0; i< transform.childCount; i++)
        {
            PhaseData phaseData = DebugUtils.GetComponentWithErrorLogging<PhaseData>(transform.GetChild(i).gameObject, "PhaseData");
            phaseDatas.Add(phaseData);
        }
    }

    public int GetMaxPhaseNum()
    {
        SetPhaseDatas();
        return phaseDatas.Count;
    }
}
