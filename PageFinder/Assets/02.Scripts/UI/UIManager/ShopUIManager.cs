using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    private List<int> scriptIdList = new();
    private ScriptData selectData;
    private PlayerScriptController playerScriptControllerScr;
    private Dictionary<int, bool> stackedScriptDataInfo = new();

    [SerializeField] private Canvas shopUICanvas;
    [SerializeField] private GameObject[] scripts;
    [SerializeField] private List<ScriptData> scriptDatas;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Button passButton;
    [SerializeField] PlayerState playerState;

    public int coinToMinus = 0;
    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }

    private void Awake()
    {
        GameObject playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(playerObj, "Player");
        passButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap));
    }

    private void OnDestroy()
    {
        passButton.onClick.RemoveAllListeners();
    }

    public void SetShopUICanvasState(bool value, bool changeScripts = true)
    {
        shopUICanvas.gameObject.SetActive(value);
        if (!value) return;

        scriptIdList.Clear();
        if (changeScripts)
        {
            SetScripts();
            coinText.text = playerState.Coin.ToString();
        }
    }

    public void SetScripts()
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            Script scriptScr = DebugUtils.GetComponentWithErrorLogging<Script>(scripts[i], "Script");
            if (!DebugUtils.CheckIsNullWithErrorLogging<Script>(scriptScr, this.gameObject))
            {
                StartCoroutine(MakeDinstinctScripts(scriptScr));
            }
        }
    }

    public IEnumerator MakeDinstinctScripts(Script scriptScr)
    {
        // 중첩이 안될때 까지
        while (true)
        {
            int index = Random.Range(0, CSVReader.Instance.AllScriptIdList.Count);
            // 스크립트 3가지 중에 한가지에 포함되어 있을 경우
            if (scriptIdList.Contains(ScriptDatas[index].scriptId))
            {
                if (scriptIdList.Count == ScriptDatas.Count) yield break;
                yield return null;
            }
            // 해당 스크립트가 플레이어한테 있을 경우
            else if (playerScriptControllerScr.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId) != null)
            {
                ScriptData playerScript = playerScriptControllerScr.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId);
                if (playerScript.level == -1 || playerScript.level >= 2) yield return null;
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
        Debug.Log("id: " + selectData.scriptId + "\nName: " + selectData.scriptName + "\nLevel: " + selectData.level + "\nType: " + selectData.scriptType);
        playerState.Coin -= selectData.price;
        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
    }
}