using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StickerDataParser : MonoBehaviour
{
    [SerializeField] private TextAsset stickerDataCsv;
    public int stickerCsvColumCounts;

    public List<StickerData> ParseSticker()
    {
        string[] data = stickerDataCsv.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / stickerCsvColumCounts - 1;
        List<StickerData> stickerDataList = new List<StickerData>();

        for (int i = 0; i < tableSize; i++)
        {
            StickerData newData = ScriptableObject.CreateInstance<StickerData>();
            newData.stickerID = int.Parse(data[stickerCsvColumCounts * (i + 1)]);
            newData.stickerName = data[stickerCsvColumCounts * (i + 1) + 1];
            newData.stickerType = SetStickerType(data[stickerCsvColumCounts * (i + 1) + 2]);
            newData.dedicatedScriptTarget = SetDedicatedScriptTarget(data[stickerCsvColumCounts * (i + 1) + 3]);
            newData.dedicatedInkType = SetDedicatedInkType(data[stickerCsvColumCounts * (i + 1) + 4]);
            newData.stickerDesc = data[stickerCsvColumCounts * (i + 1) + 5];

            newData.price = new int[4];
            newData.levelData = new float[4];
            for (int j = 0; j < 4; j++)
            {
                newData.price[j] = int.Parse(data[stickerCsvColumCounts * (i + 1) + 6 + j]);
                newData.levelData[j] = float.Parse(data[stickerCsvColumCounts * (i + 1) + 10 + j]);
            }

            stickerDataList.Add(newData);
        }

        return stickerDataList;
    }



    private StickerType SetStickerType(string stickerType)
    {
        StickerType type = StickerType.General;
        switch (stickerType)
        {
            case "Dedicated":
                type = StickerType.Dedicated;
                break;
            case "General":
                type = StickerType.General;
                break;
        }

        return type;
    }

    private DedicatedScriptTarget SetDedicatedScriptTarget(string dedicatedScriptTarget)
    {
        DedicatedScriptTarget target = DedicatedScriptTarget.None;
        switch (dedicatedScriptTarget)
        {
            case "BASICATTACK":
                target = DedicatedScriptTarget.BasicAttack;
                break;
            case "DASH":
                target = DedicatedScriptTarget.Dash;
                break;
            case "SKILL":
                target = DedicatedScriptTarget.Skill;
                break;
        }

        return target;
    }

    private InkType SetDedicatedInkType(string dedicatedInkType)
    {
        InkType type = InkType.NONE;
        switch (dedicatedInkType)
        {
            case "RED":
                type = InkType.RED;
                break;
            case "GREEN":
                type = InkType.GREEN;
                break;
            case "BLUE":
                type = InkType.BLUE;
                break;
        }

        return type;
    }
}
