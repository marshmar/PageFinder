using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemySetter : Singleton<EnemySetter>
{
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

    // 포탈 이동시, 모든 적 사망시
    public void SpawnEnemy()
    {
        List<EnemyData> enemyDatas = new List<EnemyData>();
        GameData.Instance.GetCurrEnemyDatas(ref enemyDatas);

        foreach (var enemyData in enemyDatas)
        {
            GameObject enemy = EnemyPooler.Instance.GetEnemy(enemyData.enemyType);
            enemy.transform.parent = transform;

            EnemyAction enemyScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy, "EnemyAction");
            enemyScr.InitStat(enemyData);
            CurrEnemyNums++;
        }
    }
}
