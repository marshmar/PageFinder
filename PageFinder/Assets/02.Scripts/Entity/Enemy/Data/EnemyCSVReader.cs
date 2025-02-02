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

    // Stage -> Page -> Phase
    

    public void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        
        int tableSize = data.Length / columnCounts - 1;

        for (int i = 0; i < tableSize; i++)
        {
            int index = 0;
            int dataIndex = columnCounts * (i + 1);

            EnemyData enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.id = int.Parse(data[dataIndex + index++]);
            GameData.Instance.SetStageData(ref enemyData, int.Parse(data[dataIndex + index++]), int.Parse(data[dataIndex + index++]), int.Parse(data[dataIndex + index++]));

            enemyData.rank = GetRank(data[dataIndex + index++]);
            enemyData.enemyType = GetEnemyType(data[dataIndex + index++]);
            enemyData.posType = GetPosType(data[dataIndex + index++]);
            enemyData.personality = GetPersinality(data[dataIndex + index++]);
            enemyData.patrolType = GetPatrolType(data[dataIndex + index++]);
            enemyData.inkType = GetInkType(data[dataIndex + index++]);

            enemyData.hp = int.Parse(data[dataIndex + index++]);
            enemyData.atk = int.Parse(data[dataIndex + index++]);
            enemyData.def = int.Parse(data[dataIndex + index++]);
            enemyData.cognitiveDist = int.Parse(data[dataIndex + index++]);
            enemyData.inkTypeResistance = int.Parse(data[dataIndex + index++]);
            enemyData.staggerResistance = int.Parse(data[dataIndex + index++]);

            enemyData.atkSpeed = float.Parse(data[dataIndex + index++]);
            enemyData.moveSpeed = float.Parse(data[dataIndex + index++]);
            enemyData.firstWaitTime = float.Parse(data[dataIndex + index++]);
            enemyData.attackWaitTime = float.Parse(data[dataIndex + index++]);

            enemyData.dropItem = int.Parse(data[dataIndex + index++]); // DropItem
                
            enemyData.spawnDir = GetVector3(data[dataIndex + index++].Split(" "));
            GetVector3List(ref enemyData.destinations, data[dataIndex + index++]);

            // normal Enemy

            // Elite Enemy
            GetList(ref enemyData.skillCoolTimes, data[dataIndex + index++]);
            GetList(ref enemyData.skillPriority, data[dataIndex + index++]);
            GetList(ref enemyData.skillConditions, data[dataIndex + index++]);

            // Boss Enemy

        }
    }

    Enemy.Rank GetRank(string type)
    {
        return type switch
        {
            "Low" => Enemy.Rank.MINION,
            "Normal" =>Enemy.Rank.NORMAL,
            "High" => Enemy.Rank.ELITE,
            "MiniBoss" => Enemy.Rank.BOSS,
            _ => Enemy.Rank.MINION, /* default */
        };
    }

    Enemy.EnemyType GetEnemyType(string type)
    {
        return type switch
        {
            "Jiruru" => Enemy.EnemyType.Jiruru,
            "Bansha" => Enemy.EnemyType.Bansha,
            "Witched" => Enemy.EnemyType.Witched,
            _ => Enemy.EnemyType.Jiruru, /* default */
        };
    }

    Enemy.PosType GetPosType(string type)
    {
        return type switch
        {
            "Ground" => Enemy.PosType.GROUND,
            "Sky" => Enemy.PosType.SKY,
            _ => Enemy.PosType.GROUND, /* default */
        };
    }

    Enemy.Personality GetPersinality(string type)
    {
        return type switch
        {
            "STATIC" => Enemy.Personality.STATIC,
            "CHASER" => Enemy.Personality.CHASER,
            "Avoidance" => Enemy.Personality.PATROL,
            _ => Enemy.Personality.STATIC, /* default */
        };
    }

    Enemy.PatrolType GetPatrolType(string type)
    {
        return type switch
        {
            "PATH" => Enemy.PatrolType.PATH,
            "FIX" => Enemy.PatrolType.FIX,
            _ => Enemy.PatrolType.PATH, /* default */
        };
    }

    InkType GetInkType(string type)
    {
        return type switch
        {
            "Red" => InkType.RED,
            "Green" => InkType.GREEN,
            "Blue" => InkType.BLUE,
            "Fire" => InkType.FIRE,
            "Mist" => InkType.MIST,
            "Swamp" => InkType.SWAMP,
            _ => InkType.RED, /* default */
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

    void GetList(ref List<bool> oriData, string data)
    {
        string[] list = data.Split('/');

        foreach (string s in list)
        {
            try
            {
                oriData.Add(bool.Parse(s));
            }
            catch { break; }
        }
    }

}

