using System;
using UnityEngine;

public class ScriptInventory : MonoBehaviour
{
    private BaseScript basicAttackScript;
    private BaseScript dashScript;
    private BaseScript skillScript;

    public BaseScript FindPlayerScriptByID(int scriptID)
    {
        if (basicAttackScript != null && basicAttackScript.GetID() == scriptID) return basicAttackScript;
        if (dashScript != null && dashScript.GetID() == scriptID) return dashScript;
        if (skillScript != null && skillScript.GetID() == scriptID) return skillScript;

        return null;
    }

    public void AddScript(NewScriptData scriptData)
    {
        BaseScript playerScript = FindPlayerScriptByID(scriptData.scriptID);
        if(playerScript != null)
        {
            playerScript.UpgrageScript(scriptData.rarity);
        }
        else
        {
            switch (scriptData.scriptType)
            {
                // 스크립트 팩토리 생성 필요
                // 콘텍스트 팩토리 생성 필요
                case NewScriptData.ScriptType.BasicAttack:
                    BAScript baScript = new BAScript();
                    baScript.CopyData(scriptData);
                    basicAttackScript = baScript;
                    break;
                case NewScriptData.ScriptType.Dash:
                    ChargableDashScriipt chargableDashScriipt = new ChargableDashScriipt();
                    chargableDashScriipt.CopyData(scriptData);
                    dashScript = chargableDashScriipt;
                    break;
                case NewScriptData.ScriptType.Skill:
                    ChargableSkillScript chargableSkillScript = new ChargableSkillScript();
                    skillScript.CopyData(scriptData);
                    skillScript = chargableSkillScript;
                    break;
            }
        }
    }
}
