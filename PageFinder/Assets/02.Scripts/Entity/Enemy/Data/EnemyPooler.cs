using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    [SerializeField]
    private List<EnemyType> enemyTypes = new List<EnemyType>();

    [SerializeField]
    private int defaultPoolCapacity = 10;
    [SerializeField]
    private int maxPoolSize = 50;

    private Dictionary<Enemy.EnemyType, IObjectPool<GameObject>> enemyPools = new Dictionary<Enemy.EnemyType, IObjectPool<GameObject>>();


    [SerializeField] private GameObject[] deadEffectPrefabs;


    private void Start()
    {
        foreach (var enemyType in enemyTypes)
            enemyPools[enemyType.type] = CreatePool(enemyType.prefab);
        Debug.Log("EnemyPooler 세팅 끝");
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
        if (enemy == null)
            Debug.LogError("Enemy Pooler 우선적으로 할 것");
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

        StartCoroutine(DeadEffect(type, enemy.transform.position));
    }

    IEnumerator DeadEffect(Enemy.EnemyType enemyType, Vector3 pos)
    {
        GameObject deadEffect = Instantiate(deadEffectPrefabs[Random.Range(0, deadEffectPrefabs.Length)],
            pos, Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(0.5f);

        Destroy(deadEffect);

        // 잡몹 잡았을 때만, 나머지 죽었을 때는 다른 곳에서 처리
        if(enemyType == Enemy.EnemyType.Jiruru || enemyType == Enemy.EnemyType.Bansha || enemyType == Enemy.EnemyType.Fire_Jiruru || enemyType == Enemy.EnemyType.Chaser_Jiruru)
            GameData.Instance.CurrEnemyNum -= 1;
    }
}
