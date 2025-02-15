using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static Enemy;

public class EnemySetter : Singleton<EnemySetter>
{
    public EnemyData[] enemyBasicDatas;
    private int currEnemyNums;

    public int CurrEnemyNums
    {
        get { return currEnemyNums; }
        set 
        { 
            currEnemyNums = value; 

            // 현재 웨이브의 모든 적 사망시 
            if(currEnemyNums <= 0)
            {
                currEnemyNums = 0;
                GameData.Instance.CurrWaveNum++;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            List<Vector3> destinations = new List<Vector3>();
            destinations.Add(new Vector3(6, 1.95f, 0));
            //destinations.Add(new Vector3(6, 1.95f, 0));

            EnemyData enemyData = SetEnemyData(0, Enemy.EnemyType.Jiruru, destinations);
            enemyData.hp = 500;
            List<EnemyData> enemyDatas = new List<EnemyData>();
            enemyDatas.Add(enemyData);
          
            SpawnEnemys(enemyDatas);
        }

    }

    // 포탈 이동시, 모든 적 사망시
    public void SpawnEnemys(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {
            GameObject enemy = EnemyPooler.Instance.GetEnemy(enemyData.enemyType);
            enemy.transform.parent = transform;

            EnemyAction enemyScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy, "EnemyAction");
            enemyScr.InitStat(enemyData);
            CurrEnemyNums++;
        }
    }

    /// <summary>
    /// Enemy Data Setting
    /// </summary>
    /// <param name="colNum"></param>
    /// <param name="enemyType"></param>
    /// <param name="destinations"></param>
    /// <returns></returns>
    public EnemyData SetEnemyData(int colNum, Enemy.EnemyType enemyType, List<Vector3> destinations)
    {
        EnemyData enemyData = Instantiate(enemyBasicDatas[(int)enemyType]);
        enemyData.destinations = destinations;
        return enemyData;
    }
}
