using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider bar;

    /// <summary>
    /// SliderBar의 최대 값을 조정하는 함수
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxValue)
    {
        bar.maxValue = maxValue;
        //Debug.Log(maxValue);
    }

    /// <summary>
    /// SliderBar의 현재 값을 조정하는 함수
    /// </summary>
    /// <param name="currValue"></param>
    public void SetCurrValueUI(float currValue)
    {
        if (currValue > bar.maxValue)
            Debug.LogError($"max:{bar.maxValue}    curr:{currValue}");

        bar.value = currValue;
        //Debug.Log(currValue);
    }
}
