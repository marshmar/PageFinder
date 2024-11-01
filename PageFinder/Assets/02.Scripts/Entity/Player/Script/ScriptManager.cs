using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    private PlayerScriptController playerScriptControllerScr;
    private Dictionary<int, bool> stackedScriptDataInfo;
    [SerializeField]
    private GameObject[] scripts;
    [SerializeField]
    private ScriptData[] scriptDatas;
    List<int> scriptIdList;
    private ScriptData selectData;
    [SerializeField]
    private Canvas playerOpUI;
    [SerializeField]
    private Canvas playerInfoUi;
    [SerializeField]
    private GameObject mapUi;

    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }

    private void Awake()
    {
        stackedScriptDataInfo = new Dictionary<int, bool>();
        scriptIdList = new List<int>();
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
    }
    private void OnEnable()
    {
        scriptIdList.Clear();
        SetScripts();
        if (!DebugUtils.CheckIsNullWithErrorLogging<Canvas>(playerOpUI))
        {
            playerOpUI.enabled = false;
        }
        if (!DebugUtils.CheckIsNullWithErrorLogging<Canvas>(playerInfoUi))
        {
            playerInfoUi.enabled = false;
        }
    }

    private void OnDisable()
    {

    }
    public int RandomChoice()
    {
        return Random.Range(0, scriptDatas.Length);
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
    public IEnumerator MakeDinstinctScripts(Script scriptScr)
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

        this.gameObject.SetActive(false);
    }
}
