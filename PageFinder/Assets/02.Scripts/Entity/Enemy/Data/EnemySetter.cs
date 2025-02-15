using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static Enemy;

public class EnemySetter : Singleton<EnemySetter>
{
    public EnemyData[] enemyBasicDatas;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            List<Vector3> destinations = new List<Vector3>();
            destinations.Add(new Vector3(6, 1.95f, 0));
            destinations.Add(new Vector3(-6, 1.95f, 0));

            EnemyData enemyData = SetEnemyData(0, Enemy.EnemyType.Bansha, destinations);
            enemyData.hp = 270;
            List<EnemyData> enemyDatas = new List<EnemyData>();
            enemyDatas.Add(enemyData);

            SpawnEnemys(enemyDatas);

            // Page맵에서 클릭시 아래 이벤트 호출하면 적 생성 및 UI 변경됨.
            //EventManager.Instance.PostNotification(EVENT_TYPE.PageMapToBattle, this, GameObject.Find("bg_prefab_forest_01"));
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

            Debug.Log($"{enemyData.enemyType} : {enemy.transform.position}에 소환");
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
