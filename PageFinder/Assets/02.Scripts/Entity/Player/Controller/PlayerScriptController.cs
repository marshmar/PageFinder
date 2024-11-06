using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptController : MonoBehaviour
{
    private ScriptData scriptData;
    private Player playerScr;
    private Dictionary<int, ScriptData> playerScriptDictionary;

    private int redScriptCounts;
    private int blueScriptCounts;
    private int greenScriptCounts;
    public ScriptData ScriptData { get => scriptData; set {
            scriptData = value;
            CategorizeScriptDataByTypes();
        } 
    }

    public int RedScriptCounts { get => redScriptCounts; set => redScriptCounts = value; }
    public int BlueScriptCounts { get => blueScriptCounts; set => blueScriptCounts = value; }
    public int GreenScriptCounts { get => greenScriptCounts; set => greenScriptCounts = value; }

    void Awake()
    {
        scriptData = null;
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        playerScriptDictionary = new Dictionary<int, ScriptData>();
    }


    public void CategorizeScriptDataByTypes()
    {
        //playerScriptDictionary.Add(scriptData.scriptId, scriptData);
        //IncreaseCounts();
        switch (scriptData.scriptType)
        {
            case ScriptData.ScriptType.BASICATTACK:
                Debug.Log("기본 공격 강화");
                playerScr.BasicAttackInkType = scriptData.inkType;
                break;
            case ScriptData.ScriptType.DASH:
                playerScr.DashInkType = scriptData.inkType;
                Debug.Log("대쉬 강화");
                break;
            case ScriptData.ScriptType.SKILL:
                playerScr.SkillInkType = scriptData.inkType;
                Debug.Log("스킬 강화");
                break;
            case ScriptData.ScriptType.COMMON:
                Debug.Log("기본 능력치 강화");
                break;
        }
    }

    public void IncreaseCounts()
    {
        switch (scriptData.inkType)
        {
            case InkType.RED:
                RedScriptCounts++;
                break;
            case InkType.GREEN:
                GreenScriptCounts++;
                break;
            case InkType.BLUE:
                BlueScriptCounts++;
                break;
        }
    }

    public void DecreaseCounts(ScriptData scriptData)
    {
        switch (scriptData.inkType)
        {
            case InkType.RED:
                RedScriptCounts--;
                break;
            case InkType.GREEN:
                GreenScriptCounts--;
                break;
            case InkType.BLUE:
                BlueScriptCounts--;
                break;
        }
    }
    public bool CheckScriptDataAndReturnIndex(int id)
    {
        if (playerScriptDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveScriptData(int id)
    {
        if (playerScriptDictionary.ContainsKey(id))
        {
            DecreaseCounts(playerScriptDictionary[id]);
            playerScriptDictionary.Remove(id);
            return true;
        }
        else
        {
            return false;
        }
    }
}
