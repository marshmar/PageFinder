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
    private List<ScriptData> scriptDatas;
    List<int> scriptIdList;
    List<int> allScriptIdList;
    private ScriptData selectData;

    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }
    public List<int> AllScriptIdList { get => allScriptIdList; set => allScriptIdList = value; }

    private void Awake()
    {
        stackedScriptDataInfo = new Dictionary<int, bool>();
        scriptIdList = new List<int>();
        allScriptIdList = new List<int>();
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
    }


    public void SetScriptUICanvasState(bool value)
    {
        ScriptCanvas.gameObject.SetActive(value);
        if (!value) return;

        scriptIdList.Clear();
    }

    public int RandomChoice()
    {
        return Random.Range(0, allScriptIdList.Count);
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
    // 3가지 스크립트 세팅 함수
    public IEnumerator MakeDinstinctScripts(Script scriptScr)
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
        UIManager.Instance.SetUIActiveState("PageMap");
    }
}
