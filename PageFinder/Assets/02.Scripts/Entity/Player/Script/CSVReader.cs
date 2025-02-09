using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CSVReader : Singleton<CSVReader>
{
    public TextAsset textAssetData;
    public int columnCounts;

    // 0: 기본공격,공용 1 : 대쉬 2: 스킬, 3: 잉크매직
    public Sprite[] scriptIconReds;
    public Sprite[] scriptIconGreens;
    public Sprite[] scriptIconBlues;
    // 0: Red, 1: Green, 2: Blue
    public Sprite[] scriptBackgrounds;
    // 0: 체감 온도, 1: 초목의 기운, 2: 물 절약, 3: 깊은 우물
    public Sprite[] passiveScriptIcons;

    private List<ScriptData> scriptDataList;

    private ScriptManager scriptManagerScr;
    private ShopUIManager shopUIManagerScr;
    private ScriptData playerBasicInkMagicScript;
    List<int> allScriptIdList;
    public List<int> AllScriptIdList { get => allScriptIdList; set => allScriptIdList = value; }

    private void Start()
    {
        allScriptIdList = new List<int>();
        //scriptManagerScr = DebugUtils.GetComponentWithErrorLogging<ScriptManager>(UIManager.Instance.gameObject, "ScriptManager");
        //shopUIManagerScr = DebugUtils.GetComponentWithErrorLogging<ShopUIManager>(UIManager.Instance.gameObject, "ShopUIManager");
        ReadCSV();
        scriptManagerScr.ScriptDatas = scriptDataList;
        shopUIManagerScr.ScriptDatas = scriptDataList;
        Debug.Log("CSV Reader");
    }

    void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / columnCounts - 1;
        scriptDataList = new List<ScriptData>();

        for(int i = 0; i < tableSize; i++)
        {
            scriptDataList.Add(ScriptableObject.CreateInstance<ScriptData>());
            scriptDataList[i].scriptId = int.Parse(data[columnCounts * (i + 1)]);
            scriptDataList[i].scriptName = data[columnCounts * (i + 1) + 1];
            scriptDataList[i].scriptDesc = data[columnCounts * (i + 1) + 2];
            SetInkType(ref scriptDataList[i].inkType, data[columnCounts * (i + 1) + 3]);
            SetScitptIconAndBackground(
                ref scriptDataList[i].scriptIcon,
                ref scriptDataList[i].scriptBG,
                data[columnCounts * (i + 1) + 3],
                data[columnCounts * (i + 1) + 4],
                scriptDataList[i].scriptId
            );
            SetScriptType(ref scriptDataList[i].scriptType, data[columnCounts * (i + 1) + 4]);
            scriptDataList[i].price = int.Parse(data[columnCounts * (i + 1) + 5]);
            scriptDataList[i].percentages = new float[3];
            for (int j = 0; j < 3; j++)
            {
                scriptDataList[i].percentages[j] = float.Parse(data[columnCounts * (i + 1) + 6 + j]);
            }
            SetLevelData(ref scriptDataList[i].level, scriptDataList[i].percentages[0], scriptDataList[i].percentages[1]);
            allScriptIdList.Add(scriptDataList[i].scriptId);
            // 만약 현재 스크립트 데이터의 ID가 16, 즉 열정의 불꽃일 경우 플레이어 잉크 매직의 기본 스크립트로 추가.
            if(scriptDataList[i].scriptId == 16)
            {
                Debug.Log("스크립트 id가 16인 오브젝트 찾음");
                playerBasicInkMagicScript = scriptDataList[i];
            }
        }
    }
    private void SetLevelData(ref int level, float percentage1, float percentage2)
    {
        if(percentage1 == percentage2)
        {
            level = -1;
        }
        else
        {
            level = 0;
        }
    }
    private void SetScriptType(ref ScriptData.ScriptType scriptType, string type)
    {
        switch (type)
        {
            case "BASICATTACK":
                scriptType = ScriptData.ScriptType.BASICATTACK;
                break;
            case "DASH":
                scriptType = ScriptData.ScriptType.DASH;
                break;
            case "SKILL":
                scriptType = ScriptData.ScriptType.SKILL;
                break;
            case "PASSIVE":
                scriptType = ScriptData.ScriptType.PASSIVE;
                break;
            case "MAGIC":
                scriptType = ScriptData.ScriptType.MAGIC;
                break;
        }
    }

    void SetInkType(ref InkType inktype, string type)
    {
        switch (type)
        {
            case "RED":
                inktype = InkType.RED;
                break;
            case "GREEN":
                inktype = InkType.GREEN;
                break;
            case "BLUE":
                inktype = InkType.BLUE;
                break;
        }
    }
    void SetScitptIconAndBackground(ref Sprite scriptIcon, ref Sprite scriptBackground, string inkType, string type, int scriptId)
    {
        // 체감 온도
        if(scriptId == 5)
        {
            scriptBackground = scriptBackgrounds[0];
            scriptIcon = passiveScriptIcons[0];
        }
        // 초목의 기운
        else if (scriptId == 10)
        {
            scriptBackground = scriptBackgrounds[1];
            scriptIcon = passiveScriptIcons[1];
        }
        // 물 절약
        else if(scriptId == 14)
        {
            scriptBackground = scriptBackgrounds[2];
            scriptIcon = passiveScriptIcons[2];
        }
        // 깊은 우물
        else if (scriptId == 15)
        {
            scriptBackground = scriptBackgrounds[2];
            scriptIcon = passiveScriptIcons[3];
        }
        else
        {
            switch (inkType)
            {
                case "RED":
                    scriptBackground = scriptBackgrounds[0];
                    if (type == "BASICATTACK")
                    {
                        scriptIcon = scriptIconReds[0];

                    }
                    else if (type == "DASH")
                    {
                        scriptIcon = scriptIconReds[1];
                    }
                    else if (type == "SKILL")
                    {
                        scriptIcon = scriptIconReds[2];
                    }
                    else if (type == "MAGIC")
                    {
                        scriptIcon = scriptIconReds[3];
                    }

                    break;
                case "GREEN":
                    scriptBackground = scriptBackgrounds[1];
                    if (type == "BASICATTACK")
                    {
                        scriptIcon = scriptIconGreens[0];

                    }
                    else if (type == "DASH")
                    {
                        scriptIcon = scriptIconGreens[1];
                    }
                    else if (type == "SKILL")
                    {
                        scriptIcon = scriptIconGreens[2];
                    }
                    else if (type == "MAGIC")
                    {
                        scriptIcon = scriptIconGreens[3];
                    }
                    break;
                case "BLUE":
                    scriptBackground = scriptBackgrounds[2];
                    if (type == "BASICATTACK")
                    {
                        scriptIcon = scriptIconBlues[0];
                    }
                    else if (type == "DASH")
                    {
                        scriptIcon = scriptIconBlues[1];
                    }
                    else if (type == "SKILL")
                    {
                        scriptIcon = scriptIconBlues[2];
                    }
                    else if (type == "MAGIC")
                    {
                        scriptIcon = scriptIconBlues[3];
                    }
                    break;

            }
        }
    }

    public ScriptData ReturnPlayerBasicInkMagicScript()
    {
        return playerBasicInkMagicScript;
    }
}
