using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScriptDataParser : MonoBehaviour
{
    public TextAsset scriptDataCsv;
    public TextAsset newScriptDataCsv;

    public int columnCounts;
    public int newColumnCounts;

    private List<ScriptData> scriptDataList;
    private List<NewScriptData> newScriptDataList;

    public List<ScriptData> Parse(ScriptUIMapper scriptUIMapper)
    {
        string[] data = scriptDataCsv.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / columnCounts - 1;
        scriptDataList = new List<ScriptData>();

        for (int i = 0; i < tableSize; i++)
        {
            scriptDataList.Add(ScriptableObject.CreateInstance<ScriptData>());
            scriptDataList[i].scriptId = int.Parse(data[columnCounts * (i + 1)]);
            scriptDataList[i].scriptName = data[columnCounts * (i + 1) + 1];
            scriptDataList[i].scriptDesc = data[columnCounts * (i + 1) + 2];
            SetInkType(ref scriptDataList[i].inkType, data[columnCounts * (i + 1) + 3]);
            scriptUIMapper.Map(ref scriptDataList[i].scriptIcon,
                ref scriptDataList[i].scriptBG,
                data[columnCounts * (i + 1) + 3],
                data[columnCounts * (i + 1) + 4],
                scriptDataList[i].scriptId);
            SetScriptType(ref scriptDataList[i].scriptType, data[columnCounts * (i + 1) + 4]);
            scriptDataList[i].price = int.Parse(data[columnCounts * (i + 1) + 5]);
            scriptDataList[i].percentages = new float[3];
            for (int j = 0; j < 3; j++)
            {
                scriptDataList[i].percentages[j] = float.Parse(data[columnCounts * (i + 1) + 6 + j]);
            }
            SetLevelData(ref scriptDataList[i].level, scriptDataList[i].percentages[0], scriptDataList[i].percentages[1]);
        }

        return scriptDataList;
    }

    public List<NewScriptData> ParseNew()
    {
        string[] data = newScriptDataCsv.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / newColumnCounts - 1;
        newScriptDataList = new List<NewScriptData>();

        for (int i = 0; i < tableSize; i++)
        {
            NewScriptData newData = ScriptableObject.CreateInstance<NewScriptData>();
            newData.scriptID = int.Parse(data[newColumnCounts * (i + 1)]);
            newData.scriptName = data[newColumnCounts * (i + 1) + 1];
            newData.scriptDesc = data[newColumnCounts * (i + 1) + 2];
            newData.inkType = SetInkType(data[newColumnCounts * (i + 1) + 3]);
            newData.scriptType = SetScriptType(data[newColumnCounts * (i + 1) + 4]);
            newData.price = new int[4];
            newData.levelData = new float[4];
            for(int j = 0; j < 4; j++)
            {
                newData.price[j] = int.Parse(data[newColumnCounts * (i + 1) + 5 + j]);
                newData.levelData[j] = float.Parse(data[newColumnCounts * (i + 1) + 9 + j]);
            }

            newScriptDataList.Add(newData);
        }

        return newScriptDataList;
    }


    private void SetLevelData(ref int level, float percentage1, float percentage2)
    {
        if (percentage1 == percentage2) level = -1;
        else level = 0;
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
        }
    }

    private NewScriptData.ScriptType SetScriptType(string type)
    {
        NewScriptData.ScriptType scriptType = NewScriptData.ScriptType.None;
        switch (type)
        {
            case "BASICATTACK":
                scriptType = NewScriptData.ScriptType.BasicAttack;
                break;
            case "DASH":
                scriptType = NewScriptData.ScriptType.Dash;
                break;
            case "SKILL":
                scriptType = NewScriptData.ScriptType.Skill;
                break;
            default:
                break;
        }

        return scriptType;
    }

    private void SetInkType(ref InkType inktype, string type)
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

    InkType SetInkType(string type)
    {
        InkType inkType = InkType.RED;
        switch (type)
        {
            case "RED":
                inkType = InkType.RED;
                break;
            case "GREEN":
                inkType = InkType.GREEN;
                break;
            case "BLUE":
                inkType = InkType.BLUE;
                break;
        }

        return inkType;
    }
}
