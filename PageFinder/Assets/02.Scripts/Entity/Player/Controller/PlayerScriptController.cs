using System;
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

    private PlayerDashController playerDashControllerScr;
    private PlayerSkillController playerSkillControllerScr;
    private PlayerAttackController playerAttackControllerScr;

    private ScriptData playerBasicAttacKScriptData;
    private ScriptData playerSkillScriptData;
    private ScriptData playerDashScriptData;
    private ScriptData playerMagicScriptData;

    [SerializeField]
    CSVReader csvReader;
    public ScriptData ScriptData { get => scriptData; set {
            scriptData = value;
            CategorizeScriptDataByTypes();
            IncreaseCounts();
            playerScriptDictionary.Add(scriptData.scriptId, scriptData);
        } 
    }

    public int RedScriptCounts { get => redScriptCounts; set => redScriptCounts = value; }
    public int BlueScriptCounts { get => blueScriptCounts; set => blueScriptCounts = value; }
    public int GreenScriptCounts { get => greenScriptCounts; set => greenScriptCounts = value; }
    public Dictionary<int, ScriptData> PlayerScriptDictionary { get => playerScriptDictionary; set => playerScriptDictionary = value; }
    public ScriptData PlayerMagicScriptData { get => playerMagicScriptData; set => playerMagicScriptData = value; }

    void Awake()
    {
        scriptData = null;
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        playerScriptDictionary = new Dictionary<int, ScriptData>();
        playerDashControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerController");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");

        playerBasicAttacKScriptData = null;
        playerSkillScriptData = null;
        playerDashScriptData = null;
        playerMagicScriptData = null;
}

    private IEnumerator Start()
    {
        Debug.Log("Script Player Start");

        yield return new WaitForSeconds(1);

        // 잉크 매직의 기본 스크립트를 빨강 스크립트로 설정
        ScriptData = CSVReader.Instance.ReturnPlayerBasicInkMagicScript();
    }

    public void CategorizeScriptDataByTypes()
    {
        //playerScriptDictionary.Add(scriptData.scriptId, scriptData);
        //IncreaseCounts();
        switch (scriptData.scriptType)
        {
            case ScriptData.ScriptType.BASICATTACK:
                Debug.Log("기본 공격 강화");
                if(playerBasicAttacKScriptData == null)
                {
                    playerBasicAttacKScriptData = scriptData;
                }
                else
                {
                    RemoveScriptData(playerBasicAttacKScriptData.scriptId);
                    playerBasicAttacKScriptData = scriptData;
                }
                playerScr.BasicAttackInkType = scriptData.inkType;
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
                playerScr.DashInkType = scriptData.inkType;
                if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerDashController>(playerDashControllerScr))
                {
                    playerDashControllerScr.SetDecoratorByInkType(scriptData.inkType);
                    Debug.Log(scriptData.inkType);
                }
                Debug.Log("대쉬 강화");
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
                playerScr.SkillInkType = scriptData.inkType;
                Debug.Log("스킬 강화");
                break;
            case ScriptData.ScriptType.PASSIVE:
                SetModifiers(scriptData.scriptId);
                break;
            case ScriptData.ScriptType.MAGIC:
                if (playerDashScriptData == null)
                {
                    playerMagicScriptData = scriptData;
                }
                else
                {
                    RemoveScriptData(playerMagicScriptData.scriptId);
                    playerMagicScriptData = scriptData;
                }
                playerScr.InkMagicInkType = scriptData.inkType;
                break;
        }
    }

    private void SetModifiers(int scriptId)
    {
        IStatModifier statModifier = null;
        switch (scriptId)
        {
            case 5: // 체감 온도
                statModifier = new PerceivedTemperature();
                statModifier.AddDecorator();
                break;
            case 10: // 초목의 기운
                statModifier = new EnergyOfVegetation();
                statModifier.AddDecorator();
                break;
            case 14: // 물 절약
                playerScr.WaterConservation();
                break; ;
            case 15: // 깊은 우물
                statModifier = new DeepWell();
                statModifier.AddDecorator();
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
                if(playerScr.MaxHpModifiers.Count >= 1)
                {
                    playerScr.SetMaxHP(GreenScriptCounts);
                }
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
                if (playerScr.MaxHpModifiers.Count >= 1)
                {
                    playerScr.SetMaxHP(GreenScriptCounts);
                }
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
