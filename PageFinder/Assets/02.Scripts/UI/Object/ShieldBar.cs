using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : SliderBar
{
    // 강해담 추가
    public void SetMaxValueForPlayerUI(float maxHp, float currHp, float maxShield)
    {
        if(currHp >= maxHp)
        {
            //transform.SetSiblingIndex(2);
            bar.maxValue = maxShield * 5;
        }
        else
        {
            //transform.SetSiblingIndex(1);
            bar.maxValue = maxHp;
        }
    }

    public void SetCurrValueForPlayerUI(float maxHp, float currHp, float currShield)
    {
        if(currHp >= maxHp)
        {
            bar.value = currShield;
        }
        else
        {
            Debug.Log(currShield);
            float value = currHp + currShield;
            if (value >= maxHp)
                value = maxHp;
            bar.value = value;

        }
    }
}
