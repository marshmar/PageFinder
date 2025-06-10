using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IListener
{
    void AddListener();

    // 이벤트가 발생할 때 리스너에서 호출할 함수
    void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null);
}
