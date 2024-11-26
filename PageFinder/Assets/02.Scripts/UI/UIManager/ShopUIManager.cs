using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas shopUICanvas;
    private PlayerScriptController playerScriptControllerScr;
    private Dictionary<int, bool> stackedScriptDataInfo;
    [SerializeField]
    private GameObject[] scripts;
    [SerializeField]
    private ScriptData[] scriptDatas;
    List<int> scriptIdList;
    private ScriptData selectData;

    [SerializeField]
    private TMP_Text coinText;

    [SerializeField]
    Player player;
    [SerializeField]
    PageMap pageMap;

    public int coinToMinus = 0;

    private bool isAbled;
    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }

    private void Awake()
    {
        isAbled = false;
        stackedScriptDataInfo = new Dictionary<int, bool>();
        scriptIdList = new List<int>();
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
    }


    public void SetShopUICanvasState(bool value)
    {
        shopUICanvas.gameObject.SetActive(value);

        if (!value)
            return;

        scriptIdList.Clear();
        SetScripts();
        SetCoinText();
    }


    public int RandomChoice()
    {
        return Random.Range(0, scriptDatas.Length);
    }
    public void SetScripts()
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            ShopScript scriptScr = DebugUtils.GetComponentWithErrorLogging<ShopScript>(scripts[i], "Script");
            if (!DebugUtils.CheckIsNullWithErrorLogging<ShopScript>(scriptScr, this.gameObject))
            {
                StartCoroutine(MakeDinstinctScripts(scriptScr));
            }
        }
    }
    public IEnumerator MakeDinstinctScripts(ShopScript scriptScr)
    {

        while (true)
        {
            int scriptId = RandomChoice();
            if (scriptIdList.Contains(scriptId))
            {
                if (scriptIdList.Count == scriptDatas.Length)
                {
                    yield break;
                }

                yield return null;
            }
            else
            {
                scriptIdList.Add(scriptId);
                scriptScr.ScriptData = scriptDatas[scriptId];
                yield break;
            }
        }

    }

    public void SendPlayerToScriptData()
    {
        playerScriptControllerScr.ScriptData = selectData;
        pageMap.SetPageClearData();
        player.Coin -= coinToMinus;
    }

    private void SetCoinText()
    {
        coinText.text = player.Coin.ToString();
    }
}
