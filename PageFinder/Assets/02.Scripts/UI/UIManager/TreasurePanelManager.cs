using UnityEngine;
using TMPro;

public class TreasurePanelManager : MonoBehaviour, IUIPanel
{
    [SerializeField] PlayerState playerState;
    [SerializeField] private Script treasureScript;
    [SerializeField] ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] PlayerScriptController playerScriptController;
    [SerializeField] ScriptSystemManager scriptSystemManager;
    [SerializeField] private TMP_Text coinText;

    public PanelType PanelType => PanelType.Treasure;


    public void OnClickHandler(int selection)
    {
        if (selection == 1)
        {
            SetTreasureScript(selection);
        }
        else if (selection == 2)
        {
            playerState.Coin += 80;
            EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
            proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
        }
        else if (selection == 3)
        {
            playerState.CurHp *= 0.6f;
            SetTreasureScript(selection);
        }
    }

    private void SetTreasureScript(int selection)
    {
        ScriptData scriptData;
        if (selection == 1)
        {
            scriptData = scriptSystemManager.GetRandomScriptByType(ScriptData.ScriptType.PASSIVE);
            //treasureScript.level = scriptData.level;
            treasureScript.ScriptData = scriptData;
        }
        else if (selection == 3)
        {
            while (true)
            {
                scriptData = scriptSystemManager.GetRandomScriptExcludingType(ScriptData.ScriptType.PASSIVE);
                if (playerScriptController.CheckScriptDataAndReturnIndex(scriptData.scriptId) != null) break;
            }
            //treasureScript.level = scriptData.level;
            treasureScript.ScriptData = scriptData;
        }

        treasureScript.SetScriptUI();
        ApplyScriptData();
        treasureScript.gameObject.SetActive(true);
    }

    public void OnScriptSelectedHandler()
    {
        treasureScript.gameObject.SetActive(false);
        EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
        proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        coinText.text = playerState.Coin.ToString();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void ApplyScriptData()
    {
        ScriptData scriptData = ScriptableObject.CreateInstance<ScriptData>();
        scriptData.CopyData(treasureScript.ScriptData);
        playerScriptController.ScriptData = scriptData;
        //if (selectData.level != -1) selectData.level += 1;
        Debug.Log("id: " + treasureScript.ScriptData.scriptId + "\nName: " + treasureScript.ScriptData.scriptName + "\nLevel: " + treasureScript.ScriptData.level + "\nType: " + treasureScript.ScriptData.scriptType);
    }
}
