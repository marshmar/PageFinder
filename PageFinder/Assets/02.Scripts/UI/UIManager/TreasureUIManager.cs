using TMPro;
using UnityEngine;

public class TreasureUIManager : MonoBehaviour
{
    [SerializeField] private Canvas treasureUICanvas;
    [SerializeField] PlayerState playerState;
    [SerializeField] private Script treasureScript;
    [SerializeField] private ScriptManager scriptManager;
    [SerializeField] private TMP_Text coinText;

    public void SetUICanvasState(bool value)
    {
        treasureUICanvas.gameObject.SetActive(value);
        if(value) coinText.text = playerState.Coin.ToString();
    }

    public void OnClickHandler(int selection)
    {
        if (selection == 1)
        {
            SetTreasureScript(selection);
        }
        else if (selection == 2)
        {
            playerState.Coin += 80;
            coinText.text = playerState.Coin.ToString();
            EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
        }
        else if (selection == 3)
        {
            playerState.CurHp *= 0.6f;
            SetTreasureScript(selection);
        }
    }

    private void SetTreasureScript(int selection)
    {
        treasureScript.gameObject.SetActive(true);
        if(selection == 1) treasureScript.ScriptData = CSVReader.Instance.GetRandomScriptByType(ScriptData.ScriptType.PASSIVE);
        else if(selection == 3) treasureScript.ScriptData = CSVReader.Instance.GetRandomScriptExcludingType(ScriptData.ScriptType.PASSIVE);
        scriptManager.SelectData = treasureScript.ScriptData;
        scriptManager.ApplyScriptData();
    }

    public void OnScriptSelectedHandler()
    {
        treasureScript.gameObject.SetActive(false);
        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
    }
}
