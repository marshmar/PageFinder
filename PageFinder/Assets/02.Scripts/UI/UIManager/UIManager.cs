using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    PageMapUIManager pageMapUIManager;
    ShopUIManager shopUIManager;
    CompensationUIMangaer compensationUIMangaer;
    RiddleUIManager riddleBookUIManager;
    BattleUIManager battleUIManager;
    RiddlePlayUIManager riddlePlayUIManager;


    GameObject plyaerUiOp;
    GameObject plyaerUiInfo;

    private void Start()
    {
        pageMapUIManager = gameObject.GetComponent<PageMapUIManager>();
        shopUIManager = gameObject.GetComponent<ShopUIManager>();
        compensationUIMangaer = gameObject.GetComponent<CompensationUIMangaer>();
        riddleBookUIManager = gameObject.GetComponent<RiddleUIManager>();
        riddlePlayUIManager = gameObject.GetComponent <RiddlePlayUIManager>();
        battleUIManager = gameObject.gameObject.GetComponent<BattleUIManager>();
        plyaerUiOp = GameObject.Find("Player_UI_OP");
        plyaerUiInfo = GameObject.Find("Player_UI_Info");

        SetUIActiveState("PageMap");
    }

    public void SetUIActiveState(string name)
    {
        bool active = true;
        switch (name)
        {
            case "PageMap":
                pageMapUIManager.SetPageMapUICanvasState(active);
                compensationUIMangaer.SetCompensationCanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.SetActive(!active);
                plyaerUiInfo.SetActive(!active);
                break;

            case "Compensation":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.SetActive(!active);
                plyaerUiInfo.SetActive(!active);
                break;

            case "Battle":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(!active);
                battleUIManager.SetBattleUICanvasState(active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.SetActive(active);
                plyaerUiInfo.SetActive(active);
                break;

            case "RiddleBook":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.SetActive(!active);
                plyaerUiInfo.SetActive(!active);
                break;

            case "RiddlePlay":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.SetActive(active);
                plyaerUiInfo.SetActive(active);
                break;

            case "Shop":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(!active);
                battleUIManager.SetBattleUICanvasState(!active);
                riddleBookUIManager.SetRiddleUICanvasState(!active);
                riddlePlayUIManager.SetRiddlePlayUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(active);

                plyaerUiOp.SetActive(!active);
                plyaerUiInfo.SetActive(!active);
                break;

            default:
                Debug.LogWarning(name);

                break;
        }
    }
}
