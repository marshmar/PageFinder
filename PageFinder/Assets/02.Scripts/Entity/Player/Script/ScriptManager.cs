using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    private ScriptData selectData;
    private List<ScriptData> scriptDatas;
    private PlayerScriptController playerScriptControllerScr;
    private Dictionary<int, bool> stackedScriptDataInfo = new();

    [SerializeField] private Canvas ScriptCanvas;
    [SerializeField] private GameObject[] scripts;
    [SerializeField] List<int> scriptIdList;

    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }

    [Header("Button")]
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button selectButton;

    private void Awake()
    {
        scriptIdList = new List<int>();
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
    }

    private void Start()
    {
        diaryButton.onClick.AddListener(()=>EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.RewardToDiary));
        selectButton.onClick.AddListener(() => SendPlayerToScriptData());
        selectButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap)); // ��Ż�� ������ ������ ���� : PageMap Ȱ��ȭ�� ����
    }

    public void SetScriptUICanvasState(bool value, bool changeScripts = true)
    {
        ScriptCanvas.gameObject.SetActive(value);
        if (!value) return;

        scriptIdList.Clear();
        if(changeScripts) SetScripts();
    }

    public int RandomChoice()
    {
        return Random.Range(0, CSVReader.Instance.AllScriptIdList.Count);
    }

    public void SetScripts()
    {
        for(int i = 0; i < scripts.Length; i++)
        {
            Script scriptScr = DebugUtils.GetComponentWithErrorLogging<Script>(scripts[i], "Script");
            if (!DebugUtils.CheckIsNullWithErrorLogging<Script>(scriptScr, this.gameObject))
            {
                StartCoroutine(MakeDinstinctScripts(scriptScr));
            }
        }
    }

    // 3���� ��ũ��Ʈ ���� �Լ�
    public IEnumerator MakeDinstinctScripts(Script scriptScr)
    {
        // ��ø�� �ȵ� ������
        while (true)
        {
            int index = RandomChoice();
            // ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���
            if (scriptIdList.Contains(ScriptDatas[index].scriptId))
            {
                if (scriptIdList.Count == ScriptDatas.Count) yield break;
                yield return null;
            }
        /*  else if (playerScriptControllerScr.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId) != null)
            {
                yield return null;
            }*/
            // �ش� ��ũ��Ʈ�� �÷��̾����� ���� ���
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
        //UIManager.Instance.SetUIActiveState("PageMap");
    }
}