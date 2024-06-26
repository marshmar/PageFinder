using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface VirtualJoystick : IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // 터치 시작시 1회
    public new void OnPointerDown(PointerEventData eventData);

    // 터치 상태일 때 매 프레임
    public new void OnDrag(PointerEventData eventData);

    // 터치 종료 시 1회
    public new void OnPointerUp(PointerEventData eventData);
}
