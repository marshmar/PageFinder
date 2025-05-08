using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour, IUIPanel
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

    public PanelType PanelType => PanelType.Shop;

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
        // ��ø�� �ȵɶ� ����
        while (true)
        {
            int index = Random.Range(0, CSVReader.Instance.AllScriptIdList.Count);
            // ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���
            if (scriptIdList.Contains(ScriptDatas[index].scriptId))
            {
                if (scriptIdList.Count == ScriptDatas.Count) yield break;
                yield return null;
            }
            // �ش� ��ũ��Ʈ�� �÷��̾����� ���� ���
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
            // �ش� ��ũ��Ʈ�� �÷��̾����� ����, ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���� ���
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

    public void Open()
    {
        throw new System.NotImplementedException();
    }

    public void Close()
    {
        throw new System.NotImplementedException();
    }
}