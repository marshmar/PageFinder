using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    [SerializeField]
    private Canvas ScriptCanvas;

    private PlayerScriptController playerScriptControllerScr;
    private Dictionary<int, bool> stackedScriptDataInfo;
    [SerializeField]
    private GameObject[] scripts;
    private List<ScriptData> scriptDatas;
    List<int> scriptIdList;
    private ScriptData selectData;

    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }

    [Header("Button")]
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button selectButton;
    private void Awake()
    {
        stackedScriptDataInfo = new Dictionary<int, bool>();
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
        if(changeScripts)
            SetScripts();
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
        // ��ø�� �ȵɶ� ����
        while (true)
        {
            int index = RandomChoice();
            // ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���
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
            /*// �ش� ��ũ��Ʈ�� �÷��̾����� ���� ���
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
            // �ش� ��ũ��Ʈ�� �÷��̾����� ����, ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���� ���
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
        //UIManager.Instance.SetUIActiveState("PageMap");
    }
}
