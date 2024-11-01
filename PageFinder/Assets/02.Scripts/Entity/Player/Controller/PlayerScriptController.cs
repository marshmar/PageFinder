using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptController : MonoBehaviour
{
    private ScriptData scriptData;
    private Player playerScr;

    public ScriptData ScriptData { get => scriptData; set {
            scriptData = value;
            CategorizeScriptDataByTypes();
        } 
    }

    void Awake()
    {
        scriptData = null;
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
    }


    public void CategorizeScriptDataByTypes()
    {
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

}
