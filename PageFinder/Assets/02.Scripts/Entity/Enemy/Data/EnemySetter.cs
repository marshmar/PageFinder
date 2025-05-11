using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class EnemySetter : Singleton<EnemySetter>, IListener
{
    public EnemyData[] enemyBasicDatas;

    public List<(EnemyType, GameObject)> enemies = new List<(EnemyType, GameObject)>();

    [SerializeField] BossUIManager bossUIManager;

    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
    }

    // 포탈 이동시, 모든 적 사망시
    public void SpawnEnemys(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {
            GameObject enemy = EnemyPooler.Instance.GetEnemy(enemyData.enemyType);
            enemy.transform.parent = transform;
       
            EnemyAction enemyScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy, "EnemyAction");

            if (enemyData.enemyType == EnemyType.Witched)
            {
                EnemyUI bossUi = enemyScr.GetComponent<EnemyUI>();
                bossUIManager.BindBossUI(enemyData, bossUi);
            }


            enemyScr.InitStat(enemyData);
            enemyScr.InitStatValue();
            enemyScr.StartCoroutine(enemyScr.EnemyCoroutine());
            enemies.Add((enemyData.enemyType, enemy));
           // Debug.Log($"적 : {enemyData.enemyType} 생성");


        }

        GameData.Instance.CurrEnemyNum = enemies.Count;
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
        enemyData.hp = (enemyData.hp * (1f + 0.02f * (colNum - 1)));
        enemyData.atk = (enemyData.atk * (1f + 0.02f * (colNum - 1)));

        for (int i = 0; i < enemyData.destinations.Count; i++)
            enemyData.destinations[i] += mapPos; // 맵 인스턴스에 대해 업데이트
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Stage_Clear:
                enemies.Clear();
                break;
            case EVENT_TYPE.Stage_Start:
                break;
        }
    }
}
