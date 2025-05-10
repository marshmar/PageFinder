using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour, IUIPanel
{
    public PanelType panelType;

    private ScriptData selectData;
    private PlayerScriptController playerScriptControllerScr;

    [SerializeField] private Script[] scripts;

    [SerializeField] PlayerState playerState;
    [SerializeField] ProceduralMapGenerator proceduralMapGenerator;
    [Header("Button")]
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button passButton;
    [SerializeField] private Button redrawButton;

    [Header("Text")]
    [SerializeField] private TMP_Text coinText;

    public int coinToMinus = 0;
    public ScriptData SelectData { get => selectData; set => selectData = value; }

    public PanelType PanelType => PanelType.Market;

    public bool CanDrawScripts { get => canDrawScripts; set => canDrawScripts = value; }

    private bool canDrawScripts = true;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(playerObj, "Player");

        redrawButton.onClick.AddListener(() => RedrawScripts());
        passButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD));
        passButton.onClick.AddListener(() => proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true));

        purchaseButton.onClick.AddListener(() => SendPlayerToScriptData());
    }

    private void OnDestroy()
    {
        passButton.onClick.RemoveAllListeners();
        redrawButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.RemoveAllListeners();
    }


    public void SendPlayerToScriptData()
    {
        ScriptData scriptData = ScriptableObject.CreateInstance<ScriptData>();
        scriptData.CopyData(selectData);
        playerScriptControllerScr.ScriptData = scriptData;
        //if (selectData.level != -1) selectData.level += 1;
        Debug.Log("id: " + selectData.scriptId + "\nName: " + selectData.scriptName + "\nLevel: " + selectData.level + "\nType: " + selectData.scriptType);
        playerState.Coin -= selectData.price;
        EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
        proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);

        if (canDrawScripts)
        {
            canDrawScripts = false;
            SetDistinctScripts();
            SetMarketUI();
        }

    }

    private void SetMarketUI()
    {
        coinText.text = playerState.Coin.ToString();
    }

    private void SetDistinctScripts()
    {
        var distinctScriptDatas = ScriptSystemManager.Instance.GetDistinctRandomScripts(3);
        if(distinctScriptDatas == null)
        {
            Debug.LogError("Failed to create distinctScripts");
            return;
        }

        for(int i = 0; i < scripts.Length; i++)
        {
            scripts[i].ScriptData = distinctScriptDatas[i];
            scripts[i].level = distinctScriptDatas[i].level;
            scripts[i].SetScriptUI();
        }
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void RedrawScripts()
    {
        canDrawScripts = true;
        SetDistinctScripts();
    }
}