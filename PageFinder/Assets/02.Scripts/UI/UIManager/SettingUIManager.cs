using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void SetSettingUICanvasState(bool value)
    {
        if (value)
            Time.timeScale = 0;

        settingUICanvas.SetActive(value);
        emptyBtn.SetActive(true);
        helpImg.SetActive(false);
        moveBackBtn.SetActive(false);
    }

/*    public void MoveToPageMap()
    {
        UIManager.Instance.SetUIActiveState("PageMap");
    }*/

/*    public void MoveToDiary()
    {
        UIManager.Instance.SetUIActiveState("Diary");
    }*/

    public void MoveToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void MoveToHelp()
    {
        helpImg.SetActive(true);
        moveBackBtn.SetActive(true);
        emptyBtn.SetActive(false);
    }


/*    public void MoveBack()
    {
        UIManager.Instance.SetUIActiveState("Battle");
    }*/
}
