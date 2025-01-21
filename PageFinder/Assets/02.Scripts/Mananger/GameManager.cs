using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, IListener
{
    EventManager eventManager;
    // Start is called before the first frame update
    void Start()
    {
        eventManager = EventManager.Instance;
        eventManager.AddListener(EVENT_TYPE.GAME_END, this);
    }

    public void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null)
    {
        switch (Event_Type)
        {
            case EVENT_TYPE.GAME_END:
                SceneManager.LoadScene("Title");
                break;
        }
    }
}
