using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ������ ���� �̺�Ʈ�� ��� ���.
public enum EVENT_TYPE {
    GAME_INIT, 
    GAME_END,
    INK_FUSION,

    // Joystick
    Joystick_Pressed,
    Joystick_Dragged,
    Joystick_Short_Released,
    Joystick_Long_Released,
    Joystick_Canceled,

    // Player
    InkGage_Changed,
    Generate_Shield_Player,

    // Enemy
    Generate_Shield_Enemy,

    // Buff
    Buff,

    // UI
    UI_Changed,
}

public class EventManager : Singleton<EventManager>
{
    // �̺�Ʈ ������ ������Ʈ�� ��ųʸ�(��� ������Ʈ�� �̺�Ʈ ������ ���� ��ϵǾ� ����)
    private Dictionary<EVENT_TYPE, List<IListener>> Listeners =
        new Dictionary<EVENT_TYPE, List<IListener>>();
    
    /// <summary>
    /// ������ �迭�� ������ ������ ������Ʈ�� �߰��ϱ� ���� �Լ�
    /// </summary>
    /// <param name="Event_Type">������ �̺�Ʈ</param>
    /// <param name="Listner">�̺�Ʈ�� ������ ������Ʈ</param>
    public void AddListener(EVENT_TYPE Event_Type, IListener Listner)
    {
        List<IListener> ListenList = null;

        if(Listeners.TryGetValue(Event_Type, out ListenList))
        {
            ListenList.Add(Listner);
            return;
        }

        ListenList = new List<IListener>();
        ListenList.Add(Listner);
        Listeners.Add(Event_Type, ListenList);
    }

    /// <summary>
    /// �̺�Ʈ�� �����ʿ��� �����ϱ� ���� �Լ�
    /// </summary>
    /// <param name="Event_Type">�ҷ��� �̺�Ʈ</param>
    /// <param name="Sender">�̺�Ʈ�� �θ��� ������Ʈ</param>
    /// <param name="Param">���� ������ �Ķ����</param>
    public void PostNotification(EVENT_TYPE Event_Type, Component Sender, object Param = null)
    {
        List<IListener> ListenList = null;

        if (!Listeners.TryGetValue(Event_Type, out ListenList))
            return;

        for(int i = 0; i < ListenList.Count; i++)
        {
            if (!ListenList[i].Equals(null))
                ListenList[i].OnEvent(Event_Type, Sender, Param);
        }
    }

    /// <summary>
    /// �̺�Ʈ ������ ������ �׸��� ��ųʸ����� �����Ѵ�.
    /// </summary>
    /// <param name="Event_Type">������ �̺�Ʈ</param>
    public void RemoveEvent(EVENT_TYPE Event_Type)
    { 
        Listeners.Remove(Event_Type);
    }

    /// <summary>
    /// ��ųʸ����� ������� �׸���� �����Ѵ�.
    /// </summary>
    public void RemoveRedundancies()
    {
        Dictionary<EVENT_TYPE, List<IListener>> TmpListeners
            = new Dictionary<EVENT_TYPE, List<IListener>>();

        foreach(KeyValuePair<EVENT_TYPE, List<IListener>> Item in Listeners)
        {
            for(int i = Item.Value.Count-1; i>=0; i--)
            {
                if (Item.Value[i].Equals(null))
                    Item.Value.RemoveAt(i);
            }

            if (Item.Value.Count > 0)
                TmpListeners.Add(Item.Key, Item.Value);

            Listeners = TmpListeners;
        }
    }

    /// <summary>
    /// ���� ����� �� ȣ��ȴ�. ��ųʸ��� û���Ѵ�.
    /// </summary>
    private void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }
}
