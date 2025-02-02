using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputSettings;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class UIManager : Singleton<UIManager>
{
    // 강해담 추가  - bgm 용도
    // -----------------------------------------
    [SerializeField]
    private AudioSource bgmAudioSource;
    private bool audioFirstPlay;
    // -----------------------------------------
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
        audioFirstPlay = false;
    }

    public void SetUIActiveState(string name)
    {
        prvUIName = currUIName;
        currUIName = name;

        bool active = true;
        switch (name)
        {
            case "Battle":
                if(isSetting)
                    Time.timeScale = 1;

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
                diary.SetDiaryUICanvasState(!active);

                // 강해담 추가
                if (!audioFirstPlay)
                {
                    bgmAudioSource.Play();
                    audioFirstPlay = true;
                }
                else
                {
                    bgmAudioSource.UnPause();
                }
                break;

            case "RiddleBook":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                bgmAudioSource.UnPause();
                break;

            case "Shop":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                bgmAudioSource.UnPause();
                break;

            case "Reward":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                bgmAudioSource.UnPause();
                break;

            case "Success":
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
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(active);
                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                bgmAudioSource.Pause();
                break;
            // 강해담 추가
            //------------------------------------------------
            case "Diary":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);
                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                diary.SetDiaryUICanvasState(active, "Battle");
                bgmAudioSource.Pause();
                break;

            case "Help":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);
                diary.SetDiaryUICanvasState(active, "Battle");
                bgmAudioSource.Pause();
                break;

            case "RewardToDiary":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                diary.SetDiaryUICanvasState(active, "Reward");
                bgmAudioSource.Pause();
                break;

            case "BackDiaryFromReward":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(active, false);

                success.SetActive(!active);
                defeat.SetActive(!active);
                diary.SetDiaryUICanvasState(!active);
                bgmAudioSource.UnPause();
                break;

            case "ShopToDiary":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                diary.SetDiaryUICanvasState(active, "Shop");
                bgmAudioSource.Pause();
                break;

            case "BackDiaryFromShop":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(active, false);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                diary.SetDiaryUICanvasState(!active);
                bgmAudioSource.UnPause();
                break;

            //------------------------------------------
            default:
                Debug.LogWarning("이름 잘못됨"+name);

                break;
        }
    }

    private IEnumerator RewardCoroutine(bool active)
    {
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


