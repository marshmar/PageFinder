using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static Enemy;

public class EnemySetter : Singleton<EnemySetter>
{
    public EnemyData[] enemyBasicDatas;

    private List<(EnemyType, GameObject)> enemies = new List<(EnemyType, GameObject)>();


    // 포탈 이동시, 모든 적 사망시
    public void SpawnEnemys(List<EnemyData> enemyDatas)
    {
        enemies.Clear();

        foreach (var enemyData in enemyDatas)
        {
            GameObject enemy = EnemyPooler.Instance.GetEnemy(enemyData.enemyType);
            enemy.transform.parent = transform;
       
            EnemyAction enemyScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy, "EnemyAction");
            enemyScr.InitStat(enemyData);
            enemyScr.InitStatValue();
            enemies.Add((enemyData.enemyType, enemy));
        }
        //Debug.Log($"적 {enemyDatas.Count}개 생성 끝");
    }

    /// <summary>
    /// Enemy Data Setting
    /// </summary>
    /// <param name="colNum"></param>
    /// <param name="enemyType"></param>
    /// <param name="destinations"></param>
    /// <returns></returns>
    public EnemyData SetEnemyData(Enemy.EnemyType enemyType, List<Vector3> destinations)
    {
        EnemyData newEnemy = ScriptableObject.CreateInstance<EnemyData>();
   
        newEnemy.id = enemyBasicDatas[(int)enemyType].id;
        newEnemy.rank = enemyBasicDatas[(int)enemyType].rank;
        newEnemy.enemyType = enemyBasicDatas[(int)enemyType].enemyType;
        newEnemy.posType = enemyBasicDatas[(int)enemyType].posType;
        newEnemy.personality = enemyBasicDatas[(int)enemyType].personality;
        newEnemy.patrolType = enemyBasicDatas[(int)enemyType].patrolType;
        newEnemy.attackDistType = enemyBasicDatas[(int)enemyType].attackDistType;
        newEnemy.inkType = enemyBasicDatas[(int)enemyType].inkType;


        newEnemy.hp = enemyBasicDatas[(int)enemyType].hp;
        newEnemy.atk = enemyBasicDatas[(int)enemyType].atk;
        newEnemy.cognitiveDist = enemyBasicDatas[(int)enemyType].cognitiveDist;
        newEnemy.inkTypeResistance = enemyBasicDatas[(int)enemyType].inkTypeResistance;
        newEnemy.staggerResistance = enemyBasicDatas[(int)enemyType].staggerResistance;


        newEnemy.atkSpeed = enemyBasicDatas[(int)enemyType].atkSpeed;
        newEnemy.moveSpeed = enemyBasicDatas[(int)enemyType].moveSpeed;
        newEnemy.firstWaitTime = enemyBasicDatas[(int)enemyType].firstWaitTime;
        newEnemy.attackWaitTime = enemyBasicDatas[(int)enemyType].attackWaitTime;

        newEnemy.dropItem = enemyBasicDatas[(int)enemyType].dropItem;

        newEnemy.spawnDir = enemyBasicDatas[(int)enemyType].spawnDir;
        newEnemy.destinations = destinations; // 맵 프리팹에 대한 값

        newEnemy.skillCoolTimes = enemyBasicDatas[(int)enemyType].skillCoolTimes;
        newEnemy.skillPriority = enemyBasicDatas[(int)enemyType].skillPriority;
        newEnemy.skillConditions = enemyBasicDatas[(int)enemyType].skillConditions;

        return newEnemy;
    }

    public void SetEnemyStat(EnemyData enemyData, int colNum, Vector3 mapPos)
    {
        enemyData.hp = (enemyData.hp * (1 + 0.05f * (colNum - 1)));
        enemyData.atk = (enemyData.atk * (1 + 0.1f * (colNum - 1)));

        for (int i = 0; i < enemyData.destinations.Count; i++)
            enemyData.destinations[i] += mapPos; // 맵 인스턴스에 대해 업데이트
    }

    public void RemoveAllEnemies(EnemyType enemyType)
    {
        foreach(var enemy in enemies)
            EnemyPooler.Instance.ReleaseEnemy(enemy.Item1, enemy.Item2);

        enemies.Clear();

        // 수수께끼
        // 일반 잡몹 사망시
        if (enemyType == Enemy.EnemyType.Fugitive)
            EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Goal_Fail);
        // 타겟 사망시
        else if (enemyType == Enemy.EnemyType.Target_Fugitive)
            EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Reward);
    }
}
