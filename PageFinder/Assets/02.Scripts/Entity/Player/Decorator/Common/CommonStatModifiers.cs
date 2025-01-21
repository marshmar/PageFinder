using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*// 깊은 우물 스크립트
public class DeepWell : IStatModifier
{
    Player playerScr;

    public void AddDecorator()
    {
        Debug.Log("깊은 우물 추가");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        playerScr.InkGainModifiers.Add(this);
    }
    public float ModifyStat(float inkGain)
    {
        return inkGain * 1.04f;
    }
}

// 초목의 기운 스크립트
public class EnergyOfVegetation : IStatModifier
{
    private PlayerScriptController playerScriptControllerScr;
    private Player playerScr;
    public void AddDecorator()
    {
        Debug.Log("초목의 기운 추가");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        playerScr.MaxHpModifiers.Add(this);
    }
    public float ModifyStat(float maxHP)
    {
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerScriptController>(playerScriptControllerScr))
        {
            return (playerScriptControllerScr.GreenScriptCounts * 0.04f + 1) * maxHP; 
            
        }
        // 플레이어 못 찾은 경우 maxHP만 반환(사실상 오류)
        return maxHP;
    }
}

// 체감 온도 스크립트
public class PerceivedTemperature : IStatModifier
{
    private PlayerScriptController playerScriptControllerScr;
    private Player playerScr;

    public void AddDecorator()
    {
        Debug.Log("체감 온도 추가");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        playerScr.AtkModifiers.Add(this);

    }
    public float ModifyStat(float atk)
    {
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerScriptController>(playerScriptControllerScr))
        {
            return (playerScriptControllerScr.RedScriptCounts * 0.03f + 1) * atk;
        }
        // 플레이어 못 찾은 경우 atk만 반환(사실상 오류)
        return atk;
    }
}*/
