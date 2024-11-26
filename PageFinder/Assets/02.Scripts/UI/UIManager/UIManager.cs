using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputSettings;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class UIManager : Singleton<UIManager>
{
    PageMapUIManager pageMapUIManager;
    ShopUIManager shopUIManager;
    RiddleUIManager riddleBookUIManager;
    BattleUIManager battleUIManager;
    SettingUIManager settingUIManager;

    ScriptManager reward;
    // 강해담 추가
    DiaryManager diary;

    Canvas plyaerUiOp;
    Canvas plyaerUiInfo;

    [SerializeField]
    GameObject success;
    [SerializeField]
    GameObject defeat;

    PageMap pageMap;

    bool isSetting;

    string prvUIName;
    string currUIName;

    private void Start()
    {
        pageMapUIManager = gameObject.GetComponent<PageMapUIManager>();
        shopUIManager = gameObject.GetComponent<ShopUIManager>();
        riddleBookUIManager = gameObject.GetComponent<RiddleUIManager>();
        battleUIManager = gameObject.GetComponent<BattleUIManager>();
        plyaerUiOp = DebugUtils.GetComponentWithErrorLogging<Canvas>(GameObject.Find("Player_UI_OP"), "Canvas");
        plyaerUiInfo = DebugUtils.GetComponentWithErrorLogging<Canvas>(GameObject.Find("Player_UI_Info"), "Canvas");
        reward = gameObject.GetComponent<ScriptManager>();
        settingUIManager = gameObject.GetComponent<SettingUIManager>();

        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();

        isSetting = false;

        Time.timeScale = 1;

        currUIName = "PageMap";
        SetUIActiveState(currUIName);
        diary = DebugUtils.GetComponentWithErrorLogging<DiaryManager>(this.gameObject, "DiaryManager");
        SetUIActiveState("PageMap");
    }

    public void SetUIActiveState(string name)
    {
        prvUIName = currUIName;
        currUIName = name;

        bool active = true;
        switch (name)
        {
            case "PageMap":
                pageMapUIManager.SetPageMapUICanvasState(active, prvUIName) ;
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "Battle":
                if(isSetting)
                    Time.timeScale = 1;

                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                bool isBattle = pageMap.CheckIfCurrStageIsPageToWant(Page.PageType.MIDDLEBOSS) || pageMap.CheckIfCurrStageIsPageToWant(Page.PageType.BATTLE);
                battleUIManager.SetBattleUICanvasState(active, isSetting, isBattle);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = active;
                plyaerUiInfo.enabled = active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                if (isSetting)
                    isSetting = false;
                // 강해담 추가
                // ---------------------------------
                diary.SetDiaryUICanvasState(!active);
                // ---------------------------------
                break;

            case "RiddleBook":
                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "Shop":
                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "Reward":
                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "Success":
                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(active);
                defeat.SetActive(!active);

                Invoke("LoadNextScene", 3);
                break;

            case "Defeat":
                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(active);

                Invoke("LoadNextScene", 3);
                break;

            case "Setting":
                isSetting = true;

                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(active);
            // 강해담 추가
            //------------------------------------------------
            case "PauseToDiary":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "Help":
                pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);
                diary.SetDiaryUICanvasState(active, "Battle");
                break;
            case "RewardToDiary":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;
                diary.SetDiaryUICanvasState(active, "Reward");
                break;
            case "BackDiaryToReward":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                diary.SetDiaryUICanvasState(!active);
                break;
            //------------------------------------------
            default:
                Debug.LogWarning("이름 잘못됨"+name);

                break;
        }
    }

    private IEnumerator RewardCoroutine(bool active)
    {
        pageMapUIManager.SetPageMapUICanvasState(!active, prvUIName);
        battleUIManager.SetBattleUICanvasState(!active, isSetting);
        riddleBookUIManager.SetRiddleUICanvasState(!active);
        shopUIManager.SetShopUICanvasState(!active);
        settingUIManager.SetSettingUICanvasState(!active);

        yield return new WaitForSeconds(1.0f);

        plyaerUiOp.enabled = !active;
        plyaerUiInfo.enabled = !active;
        success.SetActive(!active);
        defeat.SetActive(!active);
        reward.SetScriptUICanvasState(active);
        // 강해담 추가
        // -----------------------------
        reward.SetScripts();
        // -----------------------------
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Title");
    }
}


