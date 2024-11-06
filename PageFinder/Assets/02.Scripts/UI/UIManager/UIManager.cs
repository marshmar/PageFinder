using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class UIManager : Singleton<UIManager>
{
    PageMapUIManager pageMapUIManager;
    ShopUIManager shopUIManager;
    RiddleUIManager riddleBookUIManager;
    BattleUIManager battleUIManager;
    RiddlePlayUIManager riddlePlayUIManager;

    ScriptManager reward;
    Canvas plyaerUiOp;
    Canvas plyaerUiInfo;

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

                Debug.Log("Riddle Play Book  활성화");
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

                Debug.Log("Riddle Play UI 활성화");
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
                break;

            case "Reward":
                StartCoroutine(RewardCoroutine(active));
                Debug.Log("Reward 활성화");
                break;
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
        reward.SetScriptUICanvasState(active);
    }
}


