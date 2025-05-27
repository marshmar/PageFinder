using System;
using UnityEngine;

public class ScriptInventory : MonoBehaviour
{
    private BaseScript basicAttackScript;
    private BaseScript dashScript;
    private BaseScript skillScript;

    private NewPlayerAttackController attackController;
    private NewPlayerSkillController skillController;
    private NewPlayerDashController dashController;
    private PlayerUI playerUI;

    private void Awake()
    {
        attackController = GetComponent<NewPlayerAttackController>();
        skillController = GetComponent<NewPlayerSkillController>();
        dashController = GetComponent<NewPlayerDashController>();
        playerUI = GetComponent<PlayerUI>();



    }

    private void Start()
    {
        // 기본 공격 추가
        NewScriptData baData = ScriptableObject.CreateInstance<NewScriptData>();
        baData.CopyData(ScriptSystemManager.Instance.GetScriptDataByIDNew(1));
        AddScript(baData);

        // 대쉬 추가
        NewScriptData dashData = ScriptableObject.CreateInstance<NewScriptData>();
        dashData.CopyData(ScriptSystemManager.Instance.GetScriptDataByIDNew(8));
        AddScript(dashData);

        // 스킬 추가
        NewScriptData skillData = ScriptableObject.CreateInstance<NewScriptData>();
        skillData.CopyData(ScriptSystemManager.Instance.GetScriptDataByIDNew(6));
        AddScript(skillData);
    }

    public BaseScript FindPlayerScriptByID(int scriptID)
    {
        if (basicAttackScript != null && basicAttackScript.GetID() == scriptID) return basicAttackScript;
        if (dashScript != null && dashScript.GetID() == scriptID) return dashScript;
        if (skillScript != null && skillScript.GetID() == scriptID) return skillScript;

        return null;
    }

    public void AddScript(NewScriptData newScriptData)
    {
        BaseScript playerScript = FindPlayerScriptByID(newScriptData.scriptID);
        if(playerScript != null)
        {
            playerScript.UpgradeScript(newScriptData.rarity);
            Debug.Log("============Upgraded player script info============");
            playerScript.PrintScriptInfo();
            Debug.Log("============================================");
        }
        else
        {
            BaseScript newScript = ScriptSystemManager.Instance.CreateScriptByID(newScriptData.scriptID);
            newScript.CopyData(newScriptData);

            switch (newScript.GetScriptType())
            {
                case NewScriptData.ScriptType.BasicAttack:
                    if(basicAttackScript != null)
                        basicAttackScript.DetachAllStickers();
                    basicAttackScript = newScript;
                    attackController.SetScript(basicAttackScript);
                    playerUI.SetBasicAttackInkTypeImage(basicAttackScript.GetInkType());
                    break;
                case NewScriptData.ScriptType.Dash:
                    if(dashScript != null) 
                        dashScript.DetachAllStickers();
                    dashScript = newScript;
                    dashController.SetScript(dashScript);
                    playerUI.SetDashJoystickImage(dashScript.GetInkType());
                    break;
                case NewScriptData.ScriptType.Skill:
                    if(skillScript != null) 
                        skillScript.DetachAllStickers();
                    skillScript = newScript;
                    skillController.SetScript(skillScript);
                    playerUI.SetSkillJoystickImage(skillScript.GetInkType());
                    break;
            }

            Debug.Log("============Add Script To Player============");
            newScript.PrintScriptInfo();
            Debug.Log("============================================");
        }
    }

    public NewScriptData GetPlayerScriptDataByScriptType(NewScriptData.ScriptType scriptType)
    {
        switch (scriptType)
        {
            case NewScriptData.ScriptType.BasicAttack:
                if (basicAttackScript != null)
                    return basicAttackScript.GetCopiedData();
                break;
            case NewScriptData.ScriptType.Dash:
                if(dashScript != null)
                    return dashScript.GetCopiedData();
                break;
            case NewScriptData.ScriptType.Skill:
                if (skillScript != null)
                    return skillScript.GetCopiedData();
                break;
        }

        return null;
    }

    public BaseScript GetPlayerScriptByScriptType(NewScriptData.ScriptType scriptType)
    {
        switch (scriptType)
        {
            case NewScriptData.ScriptType.BasicAttack:
                if (basicAttackScript != null)
                    return basicAttackScript;
                break;
            case NewScriptData.ScriptType.Dash:
                if (dashScript != null)
                    return dashScript;
                break;
            case NewScriptData.ScriptType.Skill:
                if (skillScript != null)
                    return skillScript;
                break;
        }

        return null;
    }
}
