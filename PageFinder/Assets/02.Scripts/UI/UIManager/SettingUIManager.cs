using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingUIManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private Button diaryBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button goTitleBtn;

    private bool isOn = false;
    [SerializeField] private NewUIManager newUIManager;

    public PanelType PanelType => PanelType.Setting;

    private void Awake()
    { 
        diaryBtn.onClick.AddListener(MoveToDiary);
        closeBtn.onClick.AddListener(CloseSetting);
        goTitleBtn.onClick.AddListener(MoveToTitle);
    }

    private void OnDestroy()
    {
        diaryBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
        goTitleBtn.onClick.RemoveAllListeners();
    }

    public void Open()
    {
        Time.timeScale = 0f;
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void MoveToDiary()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Stacked, this, PanelType.Diary);
    }

    public void MoveToTitle()
    {
        Time.timeScale = 1f;
        EventManager.Instance.PostNotification(EVENT_TYPE.GAME_END, this);
    }

    public void CloseSetting()
    {
        isOn = false;
        Time.timeScale = 1f;
        EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
    }
}
