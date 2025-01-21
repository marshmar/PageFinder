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
    private List<ScriptData> scriptDatas;
    List<int> scriptIdList;
    private ScriptData selectData;

    [SerializeField]
    private TMP_Text coinText;

    [SerializeField]
    // 강해담 수정: player -> playerState
    //Player player;
    PlayerState playerState;
    [SerializeField]
    PageMap pageMap;

    public int coinToMinus = 0;

    private bool isAbled;
    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }

    private void Awake()
    {
        isAbled = false;
        stackedScriptDataInfo = new Dictionary<int, bool>();
        scriptIdList = new List<int>();
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
    }


    public void SetShopUICanvasState(bool value, bool changeScripts = true)
    {
        shopUICanvas.gameObject.SetActive(value);

        if (!value)
            return;

        scriptIdList.Clear();
        if (changeScripts)
        {
            SetScripts();
            SetCoinText();
        }

    }


    public int RandomChoice()
    {
        return Random.Range(0, CSVReader.Instance.AllScriptIdList.Count);
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
        // 중첩이 안될때 까지
        while (true)
        {
            int index = RandomChoice();
            // 스크립트 3가지 중에 한가지에 포함되어 있을 경우
            if (scriptIdList.Contains(ScriptDatas[index].scriptId))
            {
                if (scriptIdList.Count == ScriptDatas.Count)
                {
                    yield break;
                }

                yield return null;
            }
            else if (playerScriptControllerScr.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId) != null)
            {
                yield return null;
            }
            /*// 해당 스크립트가 플레이어한테 있을 경우
            else if (playerScriptControllerScr.CheckScriptDataAndReturnIndex(scriptId) != null)
            {
                ScriptData playerScript = playerScriptControllerScr.CheckScriptDataAndReturnIndex(scriptId);
                if(playerScript.level == -1 || playerScript.level >= 2)
                {
                    yield return null;
                }
                else
                {
                    scriptIdList.Add(scriptId);
                    playerScript.level += 1;
                    scriptScr.ScriptData = playerScriptControllerScr.CheckScriptDataAndReturnIndex(scriptId);
                }

            }*/
            // 해당 스크립트가 플레이어한테 없고, 스크립트 3가지 중에 한가지에 포함되어 있지 않을 경우
            else
            {
                scriptIdList.Add(ScriptDatas[index].scriptId);
                scriptScr.ScriptData = ScriptDatas[index];
                yield break;
            }
        }

    }

    public void SendPlayerToScriptData()
    {
        playerScriptControllerScr.ScriptData = selectData;
        playerState.Coin -= selectData.price;
        pageMap.SetPageClearData();

    }

    private void SetCoinText()
    {
        coinText.text = playerState.Coin.ToString();
    }
}
