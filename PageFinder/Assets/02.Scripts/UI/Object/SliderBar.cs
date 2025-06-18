using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    protected Slider bar;

    private void Awake()
    {
        bar = this.GetComponentSafe<Slider>();
    }

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
        bar.value = Mathf.Clamp(currValue, 0f, bar.maxValue);

/*        if (currValue > Bar.maxValue)
            Debug.LogError($"max:{Bar.maxValue}    curr:{currValue}");

        Bar.value = currValue;*/
        //Debug.Log(currValue);
    }
}
