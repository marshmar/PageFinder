using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExChangeButton : MonoBehaviour
{
    [SerializeField]
    private GameObject scriptManager;
    private Button button;
    private int exchangeCount;

    public int ExchangeCount { get => exchangeCount; set
        {
            exchangeCount = value;
            if(exchangeCount <= 0)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        exchangeCount = 1;
        button = DebugUtils.GetComponentWithErrorLogging<Button>(this.gameObject, "Button");
    }

    private void OnEnable()
    {
        exchangeCount = 1;
        button.interactable = true;
    }

    public void OnClick()
    {
        if (exchangeCount > 0)
        {
            ScriptManager scriptManagerScr = DebugUtils.GetComponentWithErrorLogging<ScriptManager>(scriptManager, "ScriptManager");
            if (!DebugUtils.CheckIsNullWithErrorLogging<ScriptManager>(scriptManagerScr, this.gameObject))
            {
                scriptManagerScr.SetScripts();
            }
            ExchangeCount--;
        }
    }

    public void OnClick2()
    {
        if (exchangeCount > 0)
        {
            ShopUIManager scriptManagerScr = DebugUtils.GetComponentWithErrorLogging<ShopUIManager>(scriptManager, "ScriptManager");
            if (!DebugUtils.CheckIsNullWithErrorLogging<ShopUIManager>(scriptManagerScr, this.gameObject))
            {
                scriptManagerScr.SetScripts();
            }
            ExchangeCount--;
        }
    }
}
