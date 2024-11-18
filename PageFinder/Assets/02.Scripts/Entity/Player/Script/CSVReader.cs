using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CSVReader : MonoBehaviour
{
    public TextAsset textAssetData;
    public int columnCounts;

    // 0: 기본공격,공용 1 : 대쉬 2: 스킬
    public Sprite[] scriptIconReds;
    public Sprite[] scriptIconGreens;
    public Sprite[] scriptIconBlues;
    // 0: Red, 1: Green, 2: Blue
    public Sprite[] scriptBackgrounds;

    private List<ScriptData> scriptDataList;

    private ScriptManager scriptManagerScr;
    private void Start()
    {
        scriptManagerScr = DebugUtils.GetComponentWithErrorLogging<ScriptManager>(UIManager.Instance.gameObject, "ScriptManager");
        ReadCSV();
        scriptManagerScr.ScriptDatas = scriptDataList;
    }

    void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / columnCounts - 1;
        scriptDataList = new List<ScriptData>();

        for(int i = 0; i < tableSize; i++)
        {
            scriptDataList.Add(new ScriptData());
            scriptDataList[i].scriptId = int.Parse(data[columnCounts * (i + 1)]);
            scriptDataList[i].scriptName = data[columnCounts * (i + 1) + 1];
            scriptDataList[i].scriptDesc = data[columnCounts * (i + 1) + 2];
            SetInkType(ref scriptDataList[i].inkType, data[columnCounts * (i + 1) + 3]);
            SetScitptIconAndBackground(
                ref scriptDataList[i].scriptIcon,
                ref scriptDataList[i].scriptBG,
                data[columnCounts * (i + 1) + 3],
                data[columnCounts * (i + 1) + 4]
            );
            scriptDataList[i].price = int.Parse(data[columnCounts * (i + 1) + 5]);
            scriptDataList[i].percentages = new float[3];
            for (int j = 0; j < 3; j++)
            {
                scriptDataList[i].percentages[j] = float.Parse(data[columnCounts * (i + 1) + 6 + j]);
            }
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
    void SetScitptIconAndBackground(ref Sprite scriptIcon, ref Sprite scriptBackground, string inkType, string type)
    {
        switch (inkType)
        {
            case "RED":
                scriptBackground = scriptBackgrounds[0];
                if (type == "BASICATTACK")
                {
                    scriptIcon = scriptIconReds[0];

                }
                else if(type == "DASH")
                {
                    scriptIcon = scriptIconReds[1];
                }
                else if(type == "SKILL")
                {
                    scriptIcon = scriptIconReds[2];
                }
                else if(type == "COMMON")
                {
                    scriptIcon = scriptIconReds[0];
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
                else if (type == "COMMON")
                {
                    scriptIcon = scriptIconGreens[0];
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
                else if (type == "COMMON")
                {
                    scriptIcon = scriptIconBlues[0];
                }
                break;

        }
    }
}
