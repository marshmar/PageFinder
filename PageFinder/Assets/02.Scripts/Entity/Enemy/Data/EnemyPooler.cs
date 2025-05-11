using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;
using static Enemy;

public class EnemyPooler : Singleton<EnemyPooler>
{
    [System.Serializable]
    private class EnemyType
    {
        public Enemy.EnemyType type;
        public GameObject prefab;
    }

    [SerializeField] private List<EnemyType> enemyTypes = new();
    [SerializeField] private int defaultPoolCapacity = 10;
    [SerializeField] private int maxPoolSize = 50;

    private Dictionary<Enemy.EnemyType, IObjectPool<GameObject>> enemyPools = new Dictionary<Enemy.EnemyType, IObjectPool<GameObject>>();

    [SerializeField] private GameObject[] deadEffectPrefabs;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] private ResultUIManager resultUIManager;
    private void Start()
    {
        foreach (var enemyType in enemyTypes)
            enemyPools[enemyType.type] = CreatePool(enemyType.prefab);
        Debug.Log("Finish the Setting of the EnemyPooler");
    }

    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultPoolCapacity,
            maxSize: maxPoolSize
        );
    }

    public GameObject GetEnemy(Enemy.EnemyType type)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogWarning(type);
            return null;
        }
        GameObject enemy = enemyPools[type].Get();
        if (enemy == null) Debug.LogError("EnemyPooler First Please");
        return enemy;
    }

    // Called when one dies or time runs out on the riddle map.
    // Action: Eliminate all current enemies, process events based on the type of enemies killed.
    public void ReleaseAllEnemy(Enemy.EnemyType type)
    {
        List<(Enemy.EnemyType, GameObject)> EnemiesCopyData = EnemySetter.Instance.enemies.ToList();

        for(int i=0; i< EnemiesCopyData.Count; i++)
        {
            // Because the boss is not in the pool
            if (EnemiesCopyData[i].Item1 == Enemy.EnemyType.Witched)
            {
                Destroy(EnemiesCopyData[i].Item2);
                continue;
            }

            ReleaseEnemy(EnemiesCopyData[i].Item1, EnemiesCopyData[i].Item2);
        }
        EnemiesCopyData.Clear();
        EnemySetter.Instance.enemies.Clear();

        // When a normal mob dies in a riddle map
        if (type == Enemy.EnemyType.Fugitive)
        {
            resultUIManager.SetResultData(ResultType.GOAL_FAIL,  1.5f);
            EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Result);
            proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
        }
        // When the target dies in the riddle map
        else if (type == Enemy.EnemyType.Target_Fugitive)
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Reward);
            proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
        }
        else if (type == Enemy.EnemyType.Witched)
        {
            resultUIManager.SetResultData(ResultType.WIN, 3f);
            EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Result);
        }
            
    }

    public void ReleaseEnemy(Enemy.EnemyType type, GameObject enemy)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogWarning(type);
            return;
        }

        EnemySetter.Instance.enemies.Remove((type, enemy));
        enemyPools[type].Release(enemy);

        StartCoroutine(DeadEffect(type, enemy.transform.position));
    }

    IEnumerator DeadEffect(Enemy.EnemyType enemyType, Vector3 pos)
    {
        GameObject deadEffect = Instantiate(deadEffectPrefabs[Random.Range(0, deadEffectPrefabs.Length)],
            pos, Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(0.5f);

        Destroy(deadEffect);

        // Only when you kill a mob, the rest of them are dealt with elsewhere when they die.
        if (enemyType == Enemy.EnemyType.Jiruru || enemyType == Enemy.EnemyType.Bansha || enemyType == Enemy.EnemyType.Fire_Jiruru || enemyType == Enemy.EnemyType.Chaser_Jiruru)
            GameData.Instance.CurrEnemyNum -= 1;
    }
}