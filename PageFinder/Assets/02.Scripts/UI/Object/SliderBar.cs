using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider Bar;

    /// <summary>
    /// SliderBar�� �ִ� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxValue)
    {
        Bar.maxValue = maxValue;
        //Debug.Log(maxValue);
    }

    /// <summary>
    /// SliderBar�� ���� ���� �����ϴ� �Լ�
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
