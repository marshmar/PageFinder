using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [SerializeField]
    protected Slider bar;

    private void Update()
    {
        transform.GetComponentInParent<Canvas>().transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
    }

    /// <summary>
    /// SliderBar의 최대 값을 조정하는 함수
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxValue)
    {
        bar.maxValue = maxValue;
    }

    /// <summary>
    /// SliderBar의 현재 값을 조정하는 함수
    /// </summary>
    /// <param name="currValue"></param>
    public void SetCurrValueUI(float currValue)
    {
        bar.value = currValue;
        Debug.Log(bar.name + bar.value);
    }
}
