using UnityEngine;
using TMPro;

public class TreasurePanelManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private bool isFixedMap = false;
    [SerializeField] private Player player;
    [SerializeField] private Script treasureScript;
    [SerializeField] ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] FixedMap fixedMap;
    [SerializeField] private TMP_Text coinText;

    public PanelType PanelType => PanelType.Treasure;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
    }
    public void OnClickHandler(int selection)
    {
        if (selection == 1)
        {
            SetTreasureScript(selection);
        }
        else if (selection == 2)
        {
            player.State.Coin += 80;
            EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
            if(isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
            else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
        }
        else if (selection == 3)
        {
            player.State.CurHp *= 0.6f;
            SetTreasureScript(selection);
        }
    }

    private void SetTreasureScript(int selection)
    {
        if (selection == 1)
        {
            StickerData stickerData = ScriptSystemManager.Instance.GetRandomStickers(1)[0];
            //treasureScript.level = scriptData.level;
            treasureScript.ScriptSystemData = stickerData;
        }
        else if (selection == 3)
        {
            NewScriptData scriptData = null;
            while (true)
            {
                int num = UnityEngine.Random.Range(0, 3);
                switch (num)
                {
                    case 0:
                        scriptData = player.ScriptInventory.GetPlayerScriptDataByScriptType(NewScriptData.ScriptType.BasicAttack);
                        break;
                    case 1:
                        scriptData = player.ScriptInventory.GetPlayerScriptDataByScriptType(NewScriptData.ScriptType.Dash);
                        break;
                    case 2:
                        scriptData = player.ScriptInventory.GetPlayerScriptDataByScriptType(NewScriptData.ScriptType.Skill);
                        break;

                }
                if (scriptData != null) break;
            }
            NewScriptData upgradedScriptData = ScriptableObject.CreateInstance<NewScriptData>();
            upgradedScriptData.CopyData(scriptData);
            upgradedScriptData.rarity += 1;
            treasureScript.ScriptSystemData = upgradedScriptData;
        }

        treasureScript.SetScriptUINew();
        ApplyScriptData();
        treasureScript.gameObject.SetActive(true);
    }

    public void OnScriptSelectedHandler()
    {
        treasureScript.gameObject.SetActive(false);
        EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
        if(isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
        else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        coinText.text = player.State.Coin.ToString();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void ApplyScriptData()
    {
        /*        NewScriptData scriptData = ScriptableObject.CreateInstance<NewScriptData>();
                scriptData.CopyData(selectDataNew);*/

        /*        Debug.Log("============Selected script info============");
                Debug.Log($"scriptID: {selectDataNew.scriptID}");
                Debug.Log($"scriptName: {selectDataNew.scriptName}");
                Debug.Log($"scriptRarity: {selectDataNew.rarity}");
                Debug.Log($"scriptMaxRarity: {selectDataNew.maxRarity}");
                Debug.Log($"scriptType: {selectDataNew.scriptType}");
                Debug.Log($"scriptInkType: {selectDataNew.inkType}");
                Debug.Log("============================================");*/

        if (treasureScript.ScriptSystemData is NewScriptData scriptData)
        {
            player.ScriptInventory.AddScript(scriptData);
        }
        else if (treasureScript.ScriptSystemData is StickerData stickerData)
        {
            player.StickerInventory.AddSticker(stickerData);
        }

    }
}
