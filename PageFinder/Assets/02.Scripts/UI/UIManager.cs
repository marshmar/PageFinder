using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    PageMapUIManager pageMapUIManager;
    ShopUIManager shopUIManager;
    CompensationUIMangaer compensationUIMangaer;
    TimerUIManager timerUIManager;
    BattleUIManager battleUIManager;

    private void Start()
    {
        pageMapUIManager = gameObject.GetComponent<PageMapUIManager>();
        shopUIManager = gameObject.GetComponent<ShopUIManager>();
        compensationUIMangaer = gameObject.GetComponent<CompensationUIMangaer>();
        timerUIManager = gameObject.GetComponent<TimerUIManager>();
        battleUIManager = gameObject.gameObject.GetComponent<BattleUIManager>();

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
                break;

            case "Compensation":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(active);
                battleUIManager.SetBattleUICanvasState(!active);
                break;

            case "Battle":
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(!active);
                battleUIManager.SetBattleUICanvasState(active);
                break;

            default:
                Debug.Log(name);
                pageMapUIManager.SetPageMapUICanvasState(!active);
                compensationUIMangaer.SetCompensationCanvasState(!active);
                break;
        }
    }
}
