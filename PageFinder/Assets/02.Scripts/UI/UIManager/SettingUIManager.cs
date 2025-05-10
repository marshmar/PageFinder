using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class SettingUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingUICanvas;

    [SerializeField]
    private GameObject helpImg;

    [SerializeField]
    private GameObject moveBackBtn;

    [SerializeField]
    private GameObject emptyBtn;

    /// <summary>
    /// 예전 활성화 버전
    /// </summary>
    /// <param name="value"></param>
    public void SetSettingUICanvasState(bool value)
    {
        if (value)
            Time.timeScale = 0;

        settingUICanvas.SetActive(value);
        emptyBtn.SetActive(true);
        helpImg.SetActive(false);
        moveBackBtn.SetActive(false);
    }

    private void OnEnable()
    {
        emptyBtn.SetActive(true);
        helpImg.SetActive(false);
        moveBackBtn.SetActive(false);
    }

    public void MoveToPageMap()
    {
        // ToDo: UI Changed;
        //EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
    }

    public void MoveToDiary()
    {
        // ToDo: UI Changed;
        //EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Diary);
    }

    public void MoveToTitle()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.GAME_END, this);
    }

    public void MoveToHelp()
    {
        helpImg.SetActive(true);
        moveBackBtn.SetActive(true);
        emptyBtn.SetActive(false);
    }


    public void MoveBack()
    {
        // ToDo: UI Changed;
        //EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
    }
}
