using System;
using UnityEngine;

public class ScriptInventory : MonoBehaviour
{
    #region Variables
    private BaseScript _basicAttackScript;
    private BaseScript _dashScript;
    private BaseScript _skillScript;

    private Player _player;
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _player = this.GetComponentSafe<Player>();
    }

    private void Start()
    {
        InitializeBasicPlayerScripts();
    }
    #endregion

    #region Initialization
    private void InitializeBasicPlayerScripts()
    {
        // Add ba Script
        NewScriptData baData = ScriptableObject.CreateInstance<NewScriptData>();
        baData.CopyData(ScriptSystemManager.Instance.GetScriptDataByIDNew(ConstantsIDs.FlameStrike));
        AddScript(baData);

        // Add dash Script
        NewScriptData dashData = ScriptableObject.CreateInstance<NewScriptData>();
        dashData.CopyData(ScriptSystemManager.Instance.GetScriptDataByIDNew(ConstantsIDs.BubbleDash));
        AddScript(dashData);

        // Add skill Script
        NewScriptData skillData = ScriptableObject.CreateInstance<NewScriptData>();
        skillData.CopyData(ScriptSystemManager.Instance.GetScriptDataByIDNew(ConstantsIDs.CottonSpores));
        AddScript(skillData);
    }

    #endregion

    #region Actions

    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion


    public BaseScript FindPlayerScriptByID(int scriptID)
    {
        if (_basicAttackScript != null && _basicAttackScript.GetID() == scriptID) return _basicAttackScript;
        if (_dashScript != null && _dashScript.GetID() == scriptID) return _dashScript;
        if (_skillScript != null && _skillScript.GetID() == scriptID) return _skillScript;

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
                    if(_basicAttackScript != null)
                        _basicAttackScript.DetachAllStickers();
                    _basicAttackScript = newScript;
                    _player.AttackController.CreateContext(_basicAttackScript);
                    _player.UI.SetBasicAttackInkTypeImage(_basicAttackScript.GetInkType());
                    break;
                case NewScriptData.ScriptType.Dash:
                    if(_dashScript != null) 
                        _dashScript.DetachAllStickers();
                    _dashScript = newScript;
                    _player.DashController.CreateContext(_dashScript);
                    _player.UI.SetDashJoystickImage(_dashScript.GetInkType());
                    break;
                case NewScriptData.ScriptType.Skill:
                    if(_skillScript != null) 
                        _skillScript.DetachAllStickers();
                    _skillScript = newScript;
                    _player.SkillController.CreateContext(_skillScript);
                    _player.UI.SetSkillJoystickImage(_skillScript.GetInkType());
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
                if (_basicAttackScript != null)
                    return _basicAttackScript.GetCopiedData();
                break;
            case NewScriptData.ScriptType.Dash:
                if(_dashScript != null)
                    return _dashScript.GetCopiedData();
                break;
            case NewScriptData.ScriptType.Skill:
                if (_skillScript != null)
                    return _skillScript.GetCopiedData();
                break;
        }

        return null;
    }

    public BaseScript GetPlayerScriptByScriptType(NewScriptData.ScriptType scriptType)
    {
        switch (scriptType)
        {
            case NewScriptData.ScriptType.BasicAttack:
                if (_basicAttackScript != null)
                    return _basicAttackScript;
                break;
            case NewScriptData.ScriptType.Dash:
                if (_dashScript != null)
                    return _dashScript;
                break;
            case NewScriptData.ScriptType.Skill:
                if (_skillScript != null)
                    return _skillScript;
                break;
        }

        return null;
    }
}
