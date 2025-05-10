using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    private ScriptData selectData;
    private List<ScriptData> scriptDatas;
    private PlayerScriptController playerScriptController;
    private Dictionary<int, bool> stackedScriptDataInfo = new();

    [SerializeField] private Canvas ScriptCanvas;
    [SerializeField] private GameObject[] scripts;
    [SerializeField] List<int> scriptIdList = new();

    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }

    public PanelType PanelType => throw new System.NotImplementedException();

    [Header("Button")]
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button selectButton;

    private void Start()
    {
        playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        // ToDo: UI Changed;
        //diaryButton.onClick.AddListener(()=>EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.RewardToDiary));
        selectButton.onClick.AddListener(() => ApplyScriptData());
        // ToDo: UI Changed;
        //selectButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap));
    }

    private void OnDestroy()
    {
        diaryButton.onClick.RemoveAllListeners();
        selectButton.onClick.RemoveAllListeners();
    }

    public void SetScriptUICanvasState(bool value, bool changeScripts = true)
    {
        ScriptCanvas.gameObject.SetActive(value);
        if (!value) return;
        scriptIdList.Clear();
        if(changeScripts) SetScripts();
    }

    public void SetScripts()
    {
        Script script;
        for(int i = 0; i < scripts.Length; i++)
        {
            script = DebugUtils.GetComponentWithErrorLogging<Script>(scripts[i], "Script");
            if (!DebugUtils.CheckIsNullWithErrorLogging<Script>(script, this.gameObject)) StartCoroutine(CreateScripts(script));
        }
    }

    private IEnumerator CreateScripts(Script script)
    {
        // ��ø�� �ȵ� ������
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
            else if (playerScriptController.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId) != null)
            {
                ScriptData playerScript = playerScriptController.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId);
                if (playerScript.level == -1 || playerScript.level >= 2) yield return null;
                else
                {
                    scriptIdList.Add(ScriptDatas[index].scriptId);
                    ScriptData scriptData = ScriptDatas[index];
                    script.level = scriptData.level;
                    script.ScriptData = scriptData;
                    yield break;
                }
            }
            // �ش� ��ũ��Ʈ�� �÷��̾����� ����, ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���� ���
            else
            {
                scriptIdList.Add(ScriptDatas[index].scriptId);
                script.level = ScriptDatas[index].level;
                script.ScriptData = ScriptDatas[index];
                yield break;
            }
        }
    }

    public void ApplyScriptData()
    {
        ScriptData scriptData = ScriptableObject.CreateInstance<ScriptData>();
        scriptData.CopyData(selectData);
        playerScriptController.ScriptData = scriptData;
        if (selectData.level != -1) selectData.level += 1;
        Debug.Log("id: " + selectData.scriptId + "\nName: " + selectData.scriptName + "\nLevel: " + selectData.level + "\nType: " + selectData.scriptType);
    }
}