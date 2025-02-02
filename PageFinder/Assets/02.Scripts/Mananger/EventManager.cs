using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 가능한 게임 이벤트를 모두 기록.
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

    // Buff
    Buff
}

public class EventManager : Singleton<EventManager>
{
    // 이벤트 리스너 오브젝트의 딕셔너리(모든 오브젝트가 이벤트 수신을 위해 등록되어 있음)
    private Dictionary<EVENT_TYPE, List<IListener>> Listeners =
        new Dictionary<EVENT_TYPE, List<IListener>>();
    
    /// <summary>
    /// 리스너 배열에 지정된 리스너 오브젝트를 추가하기 위한 함수
    /// </summary>
    /// <param name="Event_Type">수신할 이벤트</param>
    /// <param name="Listner">이벤트를 수신할 오브젝트</param>
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
    /// 이벤트를 리스너에게 전달하기 위한 함수
    /// </summary>
    /// <param name="Event_Type">불려질 이벤트</param>
    /// <param name="Sender">이벤트를 부르는 오브젝트</param>
    /// <param name="Param">선택 가능한 파라미터</param>
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
    /// 이벤트 종류와 리스너 항목을 딕셔너리에서 제거한다.
    /// </summary>
    /// <param name="Event_Type">제거할 이벤트</param>
    public void RemoveEvent(EVENT_TYPE Event_Type)
    { 
        Listeners.Remove(Event_Type);
    }

    /// <summary>
    /// 딕셔너리에서 쓸모없는 항목들을 제거한다.
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
    /// 씬이 변경될 때 호출된다. 딕셔너리를 청소한다.
    /// </summary>
    private void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }
}
