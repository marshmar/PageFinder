using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyCSVReader : Singleton<EnemyCSVReader>
{
    public TextAsset textAssetData;
    public int columnCounts;

    public EnemyData[] enemyBaiscDatas; // 기본 적 스탯 데이터들
    public List<EnemyData> enemyDatas; // CSV를 읽어서 생성한 적 데이터들

    // Stage -> Page -> Phase
    private void Start()
    {
        //ReadCSV();
    }

    public void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        
        int tableSize = data.Length / columnCounts - 1;

        for (int i = 0; i < tableSize; i++)
        {
            int index = 0;
            int dataIndex = columnCounts * (i + 1);

            int id = int.Parse(data[dataIndex + index++]);

            int enemyTypeIndex = GetEnemyTypeIndex(data[dataIndex + index++]);
            EnemyData enemyData = Instantiate(enemyBaiscDatas[enemyTypeIndex]);
            enemyData.id = id;

            GetVector3List(ref enemyData.destinations, data[dataIndex + index++]);

            // Boss Enemy

            enemyDatas.Add(enemyData);
        }
    }

    private int GetEnemyTypeIndex(string type)
    {
        return type switch
        {
            "Jiruru" => 0,
            "Bansha" => 1,
            "Witched" => 2,
            _ => 0, /* default */
        };
    }

    Vector3 GetVector3(string[] data)
    {
        float[] nums = new float[3];

        for (int i = 0; i < nums.Length; i++)
            nums[i] = float.Parse(data[i]);

        return new Vector3(nums[0], nums[1], nums[2]);
    }

    void GetVector3List(ref List<Vector3> oriData, string v3Data)
    {
        /*  
         * CSV 저장 형식 : 0 0 0/1 1 1
         * Unity : (0,0,0) (1,1,1) 
         */

        string[] v3List = v3Data.Split('/');

        for(int cnt = 0; cnt < v3List.Length; cnt++)
        {
            string[] pos = v3List[cnt].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            oriData.Add(GetVector3(pos));
        }
    }

    void GetList(ref List<int> oriData, string data)
    {
        string[] list = data.Split('/', StringSplitOptions.RemoveEmptyEntries);
        foreach(string s in list)
        {
            if (s.Equals(""))
                break;
            oriData.Add(int.Parse(s));
        }
    }

    void GetList(ref List<float> oriData, string data)
    {
        string[] list = data.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (string s in list)
        {
            if (s.Equals(""))
                break;
            oriData.Add(float.Parse(s));
        }
    }
}

