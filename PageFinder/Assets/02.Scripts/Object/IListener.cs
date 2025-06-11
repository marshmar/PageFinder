using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IListener
{
    void AddListener();

    void RemoveListener();

    // �̺�Ʈ�� �߻��� �� �����ʿ��� ȣ���� �Լ�
    void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null);
}
