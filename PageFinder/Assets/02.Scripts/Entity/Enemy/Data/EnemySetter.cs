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

            EnemyData enemyData = SetEnemyData(0, Enemy.EnemyType.Jiruru, destinations);
            List<EnemyData> enemyDatas = new List<EnemyData>();
            enemyDatas.Add(enemyData);

            SpawnEnemys(enemyDatas);

            // Page�ʿ��� Ŭ���� �Ʒ� �̺�Ʈ ȣ���ϸ� �� ���� �� UI �����.
            //EventManager.Instance.PostNotification(EVENT_TYPE.PageMapToBattle, this, GameObject.Find("bg_prefab_forest_01"));
        }

    }

    // ��Ż �̵���, ��� �� �����
    public void SpawnEnemys(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {
            GameObject enemy = EnemyPooler.Instance.GetEnemy(enemyData.enemyType);
            enemy.transform.parent = transform;
       
            EnemyAction enemyScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy, "EnemyAction");
            enemyScr.InitStat(enemyData);

            Debug.Log($"{enemyData.enemyType} : {enemy.transform.position}�� ��ȯ");
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
