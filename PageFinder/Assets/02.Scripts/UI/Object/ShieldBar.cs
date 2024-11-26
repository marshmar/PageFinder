using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : SliderBar
{
    /// <summary>
    /// SliderBar의 최대 값을 조정하는 함수
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxHp, float currHp, float maxShield)
    {
        bar.maxValue = maxShield;

        // 현재 체력바 바로 오른쪽에 위치하도록 조정
        bar.transform.localPosition = new Vector3(-1f + 2 * currHp / (maxHp + maxShield), transform.localPosition.y, transform.localPosition.z);
        bar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100 * maxShield / (maxShield + maxHp));
    }

    // 강해담 추가
    public void SetMaxValueUI(float maxShield)
    {
        bar.maxValue = maxShield;
    }
}
