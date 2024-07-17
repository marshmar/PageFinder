using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 가능한 게임 이벤트를 모두 기록.
public enum EVENT_TYPE {
    GAME_INIT, 
    GAME_END
}

public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// 이벤트를 위한 델리게이트 형식을 선언한다.
    /// </summary>
    /// <param name="Event_Type">발생한 이벤트 종류</param>
    /// <param name="Sender">이벤트 발생 확인 송신자</param>
    /// <param name="Param">선택 가능한 파라미터</param>
    public delegate void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null);

    // 이벤트 리스너 오브젝트의 딕셔너리(모든 오브젝트가 이벤트 수신을 위해 등록되어 있음)
    private Dictionary<EVENT_TYPE, List<OnEvent>> Listeners =
        new Dictionary<EVENT_TYPE, List<OnEvent>>();
    
    /// <summary>
    /// 리스너 배열에 지정된 리스너 오브젝트를 추가하기 위한 함수
    /// </summary>
    /// <param name="Event_Type">수신할 이벤트</param>
    /// <param name="Listner">이벤트를 수신할 오브젝트</param>
    public void AddListener(EVENT_TYPE Event_Type, OnEvent Listner)
    {
        // 이 이벤트를 수신할 리스너의 리스트
        List<OnEvent> ListenList = null;

        // 이벤트 형식 키가 존재하는지 검사하고 존재하면 리스트에 추가한다.
        if(Listeners.TryGetValue(Event_Type, out ListenList))
        {
            // 리스트가 존재하면 새 항목을 추가한다
            ListenList.Add(Listner);
            return;
        }

        // 리스트가 존재하지 않으면 새로운 리스트를 생성한다.
        ListenList = new List<OnEvent>();
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
        // 모든 리스너에게 이벤트에 대해 알린다.

        // 이 이벤트를 수신하는 리스너들의 리스트
        List<OnEvent> ListenList = null;

        // 이벤트 항목이 없으면, 알릴 리스너가 없으므로 끝낸다.
        if (!Listeners.TryGetValue(Event_Type, out ListenList))
            return;

        // 항목이 존재하면 적합한 리스너에게 알려준다.
        for(int i = 0; i < ListenList.Count; i++)
        {
            // 오브젝트가 null이 아니면 인터페이스를 통해 메시지를 보낸다.
            if (!ListenList[i].Equals(null))
                ListenList[i](Event_Type, Sender, Param);
        }
    }

    /// <summary>
    /// 이벤트 종류와 리스너 항목을 딕셔너리에서 제거한다.
    /// </summary>
    /// <param name="Event_Type">제거할 이벤트</param>
    public void RemoveEvent(EVENT_TYPE Event_Type)
    {
        // 딕셔너리의 항목을 제거한다.
        Listeners.Remove(Event_Type);
    }

    /// <summary>
    /// 딕셔너리에서 쓸모없는 항목들을 제거한다.
    /// </summary>
    public void RemoveRedundancies()
    {
        // 새 딕셔너리 생성
        Dictionary<EVENT_TYPE, List<OnEvent>> TmpListeners
            = new Dictionary<EVENT_TYPE, List<OnEvent>>();

        // 모든 딕셔너리 항목을 순회한다
        foreach(KeyValuePair<EVENT_TYPE, List<OnEvent>> Item in Listeners)
        {
            // 리스트의 모든 리스너 오브젝트를 순회하며 null 오브젝트를 제거한다
            for(int i = Item.Value.Count-1; i>=0; i--)
            {
                // null이면 항목을 지운다
                if (Item.Value[i].Equals(null))
                    Item.Value.RemoveAt(i);
            }

            // 알림을 받기 위한 항목들만 리스트에 남으면 이 항목들을 임시 딕셔너리에 담는다.
            if (Item.Value.Count > 0)
                TmpListeners.Add(Item.Key, Item.Value);

            // 새로 최적화된 딕셔너리로 교체한다.
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
