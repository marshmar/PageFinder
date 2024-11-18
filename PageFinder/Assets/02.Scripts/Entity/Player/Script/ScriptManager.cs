using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    [SerializeField]
    private Canvas ScriptCanvas;

    private PlayerScriptController playerScriptControllerScr;
    private Dictionary<int, bool> stackedScriptDataInfo;
    [SerializeField]
    private GameObject[] scripts;
    [SerializeField]
    private List<ScriptData> scriptDatas;
    List<int> scriptIdList;
    private ScriptData selectData;

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


    public void SetScriptUICanvasState(bool value)
    {
        ScriptCanvas.gameObject.SetActive(value);
        if (!value) return;

        scriptIdList.Clear();
        SetScripts();
    }

    public int RandomChoice()
    {
        return Random.Range(0, ScriptDatas.Count);
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
                if (scriptIdList.Count == ScriptDatas.Count)
                {
                    yield break;
                }

                yield return null;
            }
            else
            {
                scriptIdList.Add(scriptId);
                scriptScr.ScriptData = ScriptDatas[scriptId];
                yield break;
            }
        }

    }

    public void SendPlayerToScriptData()
    {
        playerScriptControllerScr.ScriptData = selectData;
        UIManager.Instance.SetUIActiveState("PageMap");
    }
}
