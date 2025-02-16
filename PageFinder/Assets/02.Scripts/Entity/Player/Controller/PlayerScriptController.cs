using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptController : MonoBehaviour
{
    private ScriptData scriptData;
    //private Player playerScr;
    private PlayerInkType playerInkType;
    [SerializeField] private Dictionary<int, ScriptData> playerScriptDictionary;

    private int redScriptCounts;
    private int blueScriptCounts;
    private int greenScriptCounts;

    private PlayerDashController playerDashControllerScr;
    private PlayerSkillController playerSkillControllerScr;
    private PlayerAttackController playerAttackControllerScr;
    private PlayerBuff playerBuff;
    private PlayerState playerState;

    private ScriptData playerBasicAttacKScriptData;
    private ScriptData playerSkillScriptData;
    private ScriptData playerDashScriptData;

    private bool perceivedTemperature = false;
    private bool energyOfVegetation = false;
    public ScriptData ScriptData { get => scriptData; set {
            scriptData = value;
            CategorizeScriptDataByTypes();
            IncreaseCounts();
            AddScriptDataToDictionary(scriptData);
            
        } 
    }

    private void AddScriptDataToDictionary(ScriptData scriptData)
    {
        if (playerScriptDictionary.ContainsKey(scriptData.scriptId))
        {
            playerScriptDictionary[scriptData.scriptId] = scriptData;
        }
        else
        {
            playerScriptDictionary.Add(scriptData.scriptId, scriptData);
        }
    }

    public int RedScriptCounts { get => redScriptCounts; 
        set 
        { redScriptCounts = value; 

        } 
    }
    public int BlueScriptCounts { get => blueScriptCounts; set => blueScriptCounts = value; }
    public int GreenScriptCounts { get => greenScriptCounts; set => greenScriptCounts = value; }
    public Dictionary<int, ScriptData> PlayerScriptDictionary { get => playerScriptDictionary; set => playerScriptDictionary = value; }
    public ScriptData PlayerSkillScriptData { get => playerSkillScriptData; set => playerSkillScriptData = value; }

    void Awake()
    {
        scriptData = null;
        playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(this.gameObject, "PlayerInkType");
        playerScriptDictionary = new Dictionary<int, ScriptData>();
        playerDashControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerController");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerBuff = DebugUtils.GetComponentWithErrorLogging<PlayerBuff>(this.gameObject, "PlayerBuff");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerBasicAttacKScriptData = null;
        playerSkillScriptData = null;
        playerDashScriptData = null;
}

    private IEnumerator Start()
    {
        Debug.Log("Script Player Start");

        yield return new WaitForSeconds(1);
    }

    public void CategorizeScriptDataByTypes()
    {
        //playerScriptDictionary.Add(scriptData.scriptId, scriptData);
        //IncreaseCounts();
        switch (scriptData.scriptType)
        {
            case ScriptData.ScriptType.BASICATTACK:
                if(playerBasicAttacKScriptData == null)
                {
                    playerBasicAttacKScriptData = scriptData;
                }
                else
                {
                    RemoveScriptData(playerBasicAttacKScriptData.scriptId);
                    playerBasicAttacKScriptData = scriptData;
                }
                playerInkType.BasicAttackInkType = scriptData.inkType;
                if(scriptData.inkType == InkType.RED)
                {
                    EventManager.Instance.PostNotification(EVENT_TYPE.Create_Script, this, new System.Tuple<int, float>(scriptData.scriptId, scriptData.percentages[scriptData.level]));
                }
                if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerAttackController>(playerAttackControllerScr))
                {
                    //playerAttackController.SetDecorator();
                }
                break;
            case ScriptData.ScriptType.DASH:
                if (playerDashScriptData == null)
                {
                    playerDashScriptData = scriptData;
                }
                else
                {
                    RemoveScriptData(playerDashScriptData.scriptId);
                    playerDashScriptData = scriptData;
                }
                playerInkType.DashInkType = scriptData.inkType;
                if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerDashController>(playerDashControllerScr))
                {
                    playerDashControllerScr.SetDecoratorByInkType(scriptData.inkType, scriptData.percentages[scriptData.level]);
                    Debug.Log(scriptData.inkType);
                }
                break;
            case ScriptData.ScriptType.SKILL:
                if (playerSkillScriptData == null)
                {
                    playerSkillScriptData = scriptData;
                }
                else
                {
                    RemoveScriptData(playerSkillScriptData.scriptId);
                    playerSkillScriptData = scriptData;
                }
                playerInkType.SkillInkType = scriptData.inkType;
                if(scriptData.inkType == InkType.RED)
                {
                    playerSkillControllerScr.fireWork = true;
                    playerSkillControllerScr.fireWorkValue = scriptData.percentages[scriptData.level];
                }
                break;
            case ScriptData.ScriptType.PASSIVE:
                EventManager.Instance.PostNotification(EVENT_TYPE.Create_Script, this, new System.Tuple<int, float>(scriptData.scriptId, scriptData.percentages[scriptData.level != -1 ?  scriptData.level : 0]));
                if (scriptData.scriptId == 5) perceivedTemperature = true;
                if (scriptData.scriptId == 10) energyOfVegetation = true;
                break;
        }
    }

    public void IncreaseCounts()
    {
        switch (scriptData.inkType)
        {
            case InkType.RED:
                RedScriptCounts++;
                if (perceivedTemperature) playerState.PerceivedTemperature(RedScriptCounts);
                break;
            case InkType.GREEN:
                GreenScriptCounts++;
                if (energyOfVegetation) playerState.EnergyOfVegetation(GreenScriptCounts);
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
    public ScriptData CheckScriptDataAndReturnIndex(int id)
    {
        if (playerScriptDictionary.ContainsKey(id))
        {
            return playerScriptDictionary[id];
        }
        else
        {
            return null;
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
