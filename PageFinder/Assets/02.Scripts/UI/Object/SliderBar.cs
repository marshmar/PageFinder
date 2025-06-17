using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider Bar;

    /// <summary>
    /// SliderBar의 최대 값을 조정하는 함수
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxValue)
    {
        Bar.maxValue = maxValue;
        //Debug.Log(maxValue);
    }

    /// <summary>
    /// SliderBar의 현재 값을 조정하는 함수
    /// </summary>
    /// <param name="currValue"></param>
    public void SetCurrValueUI(float currValue)
    {
        Bar.value = Mathf.Clamp(currValue, 0f, Bar.maxValue);

/*        if (currValue > Bar.maxValue)
            Debug.LogError($"max:{Bar.maxValue}    curr:{currValue}");

        Bar.value = currValue;*/
        //Debug.Log(currValue);
    }
}
