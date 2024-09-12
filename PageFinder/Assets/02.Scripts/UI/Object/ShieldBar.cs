using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBar : SliderBar
{
    /// <summary>
    /// SliderBar의 최대 값을 조정하는 함수
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxShieldValueUI(float maxHp, float currHp, float maxShield)
    {
        // 현재 체력바 바로 오른쪽에 위치하도록 조정
        bar.transform.position = new Vector3(-1f + 2 * currHp / (maxHp + maxShield), transform.position.y, transform.position.z);

        // shield Bar는 무조건 최대 값 1을 가지고 꽉 채워서 시작한다. 
        bar.maxValue = maxShield;

        bar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100 * maxShield / (maxShield + maxHp));
    }
}
