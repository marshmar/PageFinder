using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
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

    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }

    [SerializeField]
    private TMP_Text coinText;

    [SerializeField]
    PlayerState playerState;

    public int coinToMinus = 0;

    [SerializeField]
    private Button passButton;

    private void Awake()
    {
        stackedScriptDataInfo = new Dictionary<int, bool>();
        scriptIdList = new List<int>();

        GameObject playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(playerObj, "Player");
        passButton.onClick.AddListener(Pass);
    }

    public void SetShopUICanvasState(bool value, bool changeScripts = true)
    {
        shopUICanvas.gameObject.SetActive(value);

        if (!value) return;

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
            // 해당 스크립트가 플레이어한테 있을 경우
            else if (playerScriptControllerScr.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId) != null)
            {
                ScriptData playerScript = playerScriptControllerScr.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId);
                if (playerScript.level == -1 || playerScript.level >= 2)
                {
                    yield return null;
                }
                else
                {
                    scriptIdList.Add(ScriptDatas[index].scriptId);
                    ScriptData scriptData = ScriptDatas[index];
                    scriptScr.level = scriptData.level;
                    scriptScr.ScriptData = scriptData;
                    yield break;
                }

            }
            // 해당 스크립트가 플레이어한테 없고, 스크립트 3가지 중에 한가지에 포함되어 있지 않을 경우
            else
            {
                scriptIdList.Add(ScriptDatas[index].scriptId);
                scriptScr.level = ScriptDatas[index].level;
                scriptScr.ScriptData = ScriptDatas[index];
                yield break;
            }
        }

    }

    public void SendPlayerToScriptData()
    {
        ScriptData scriptData = ScriptableObject.CreateInstance<ScriptData>();
        scriptData.CopyData(selectData);
        playerScriptControllerScr.ScriptData = scriptData;
        if (selectData.level != -1) selectData.level += 1;

        playerState.Coin -= selectData.price;

        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
    }

    private void SetCoinText()
    {
        coinText.text = playerState.Coin.ToString();
    }

    private void Pass()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
    }
}
