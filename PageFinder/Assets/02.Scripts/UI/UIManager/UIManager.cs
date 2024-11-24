using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class UIManager : Singleton<UIManager>
{
    PageMapUIManager pageMapUIManager;
    ShopUIManager shopUIManager;
    RiddleUIManager riddleBookUIManager;
    BattleUIManager battleUIManager;
    RiddlePlayUIManager riddlePlayUIManager;

    ScriptManager reward;
    // 강해담 추가
    DiaryManager diary;

    Canvas plyaerUiOp;
    Canvas plyaerUiInfo;

    [SerializeField]
    GameObject success;
    [SerializeField]
    GameObject defeat;

    private void Start()
    {
        pageMapUIManager = gameObject.GetComponent<PageMapUIManager>();
        shopUIManager = gameObject.GetComponent<ShopUIManager>();
        riddleBookUIManager = gameObject.GetComponent<RiddleUIManager>();
        riddlePlayUIManager = gameObject.GetComponent <RiddlePlayUIManager>();
        battleUIManager = gameObject.GetComponent<BattleUIManager>();
        plyaerUiOp = DebugUtils.GetComponentWithErrorLogging<Canvas>(GameObject.Find("Player_UI_OP"), "Canvas");
        plyaerUiInfo = DebugUtils.GetComponentWithErrorLogging<Canvas>(GameObject.Find("Player_UI_Info"), "Canvas");
        reward = gameObject.GetComponent<ScriptManager>();
        diary = DebugUtils.GetComponentWithErrorLogging<DiaryManager>(this.gameObject, "DiaryManager");
        SetUIActiveState("PageMap");
    }

    public void SetUIActiveState(string name)
    {
        bool active = true;
        switch (name)
        {
            case "PageMap":
                pageMapUIManager.SetPageMapUICanvasState(active);
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

            case "Battle":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = active;
                plyaerUiInfo.enabled = active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                // 강해담 추가
                // ---------------------------------
                diary.SetDiaryUICanvasState(!active);
                // ---------------------------------
                break;

            case "RiddleBook":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "RiddlePlay":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = active;
                plyaerUiInfo.enabled = active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "Shop":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(!active);
                break;

            case "Reward":
                StartCoroutine(RewardCoroutine(active));
                Debug.Log("Reward 활성화");
                break;

            case "Success":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(active);
                defeat.SetActive(!active);

                Invoke("LoadNextScene", 3);
                break;

            case "Defeat":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                success.SetActive(!active);
                defeat.SetActive(active);

                Invoke("LoadNextScene", 3);
                break;

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
        pageMapUIManager.SetPageMapUICanvasState(!active);
        battleUIManager.SetBattleUICanvasState(!active);
        riddleBookUIManager.SetRiddleUICanvasState(!active);
        riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
        shopUIManager.SetShopUICanvasState(!active);



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


