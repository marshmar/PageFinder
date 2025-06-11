using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, IListener
{
    void Start()
    {
        AddListener();
    }

    void OnDestroy()
    {
        RemoveListener();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Setting);
        }
    }

    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.GAME_END, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.GAME_END, this);
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