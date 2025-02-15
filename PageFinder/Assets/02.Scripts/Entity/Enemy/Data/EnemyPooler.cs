using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;

public class EnemyPooler : Singleton<EnemyPooler>
{
    [System.Serializable]
    private class EnemyType
    {
        public Enemy.EnemyType type;
        public GameObject prefab;
    }

    [SerializeField]
    private List<EnemyType> enemyTypes = new List<EnemyType>();

    [SerializeField]
    private int defaultPoolCapacity = 10;
    [SerializeField]
    private int maxPoolSize = 50;

    private Dictionary<Enemy.EnemyType, IObjectPool<GameObject>> enemyPools = new Dictionary<Enemy.EnemyType, IObjectPool<GameObject>>();


    private void Start()
    {
        foreach (var enemyType in enemyTypes)
            enemyPools[enemyType.type] = CreatePool(enemyType.prefab);
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
        return enemy;
    }

    public void ReleaseEnemy(Enemy.EnemyType type, GameObject enemy)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogWarning(type);
            return;
        }

        enemyPools[type].Release(enemy);
    }
}
