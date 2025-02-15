using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Playables;

public class RiddleCSVReader : Singleton<RiddleCSVReader>
{
    [SerializeField]
    private TextAsset textAssetData;
    [SerializeField]
    private int columnCounts;

    private List<RiddleData> riddleDatas = new List<RiddleData>();

    private void Start()
    {
        ReadCSV();
        Debug.Log("Riddle CSV Read");
    }

    public void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / columnCounts - 1;

        for (int i = 0; i < tableSize; i++)
        {
            int index = 0;
            int dataIndex = columnCounts * (i + 1);

            RiddleData riddleData = ScriptableObject.CreateInstance<RiddleData>();

            int id = int.Parse(data[dataIndex + index++]);
            riddleData.stageNum = int.Parse(data[dataIndex + index++]);
            riddleData.positiveConversation =  GetString(data[dataIndex + index++]);
            riddleData.neagativeConversation =  GetString(data[dataIndex + index++]);

            riddleData.problem = GetString(data[dataIndex + index++]);
            riddleData.options = data[dataIndex + index++].Split("/", StringSplitOptions.RemoveEmptyEntries);

            SetStrings(ref riddleData.conversations, data[dataIndex + index++]);
            SetStrings(ref riddleData.conversations, data[dataIndex + index++]);
            riddleDatas.Add(riddleData);
        }
    }

    public void SetStrings(ref List<string> oriStrings, string s)
    {
        string[] datas = s.Split("/", StringSplitOptions.RemoveEmptyEntries);
        string pageData = "";

        foreach (string data in datas)
        {
            if (data.Equals(" "))
                pageData += "\n";
            else
                pageData += data + "\n";
        }

        oriStrings.Add(pageData);
    }

    public string GetString(string s)
    {
        string[] datas = s.Split("/", StringSplitOptions.RemoveEmptyEntries);
        string pageData = "";

        foreach (string data in datas)
        {
            if (data.Equals(" "))
                pageData += "\n";
            else
                pageData += data;
        }

        return pageData;
    }

    public RiddleData GetRiddleData(int stageNum)
    {
        if (riddleDatas == null || stageNum > riddleDatas.Count)
            return null;

        return riddleDatas[stageNum-1];
    }

}
