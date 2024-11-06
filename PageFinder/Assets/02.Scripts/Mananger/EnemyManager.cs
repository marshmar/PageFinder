using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;


public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]
    private GameObject[] Enemies_Prefab;

    [SerializeField]
    private GameObject[] Fugitive_Prefab;

    private List<GameObject> Enemies = new List<GameObject>();

    [SerializeField]
    private GameObject enemyHPCanvas_Prefab;
    [SerializeField]
    private GameObject targetEnemyHPCanvas_Prefab;


    PageMap pageMap;
    private void Start()
    {
        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();
        CreateRooms(1);
    }


    /// <summary>
    /// 최대 방의 개수만큼 EnemyManager의 자식 객체로 생성한다.
    /// </summary>
    /// <param name="maxMapNum">방의 최대 개수</param>
    public void CreateRooms(int maxMapNum)
    {
        GameObject map;
        for (int i=0; i< maxMapNum; i++)
        {
            map = new GameObject((i+1).ToString());
            map.transform.parent = transform;
        }
    }

    /// <summary>
    /// 적을 생성한다.
    /// </summary>
    /// <param name="mapNum">적이 생성되는 맵 번호</param>
    /// <param name="type">적의 종류</param>
    /// <param name="pos">적 생성 위치</param>
    /// <returns>현재 만든 적의 이름</returns>
    public string CreateEnemy(int mapNum, string type, Vector3 spawnPos, Vector3 dir, int moveDist = 5, bool isTargetEnemy = false, bool isBossStage = false)
    {
        int index = 0;
        GameObject obj = null;
        mapNum--;

        switch (type)
        {
            case "Jiruru":
                index = 0;
                break;
            case "Bansha":
                index = 1;
                break;
            case "Witched":
                index = 2;
                break;

            default:
                Debug.LogWarning(type);
                break;
        }


        if(isTargetEnemy)
        {
            obj = Instantiate(Enemies_Prefab[index], spawnPos, Quaternion.Euler(dir), transform.GetChild(mapNum));
            Instantiate(targetEnemyHPCanvas_Prefab, Vector3.zero, Quaternion.identity, obj.transform);
            if (!type.Equals("Witched"))
                obj.transform.localScale = Vector3.one * 1.5f;
            obj.name = "Target-" + type;
        }
        else
        {
            obj = Instantiate(Enemies_Prefab[index], spawnPos, Quaternion.Euler(dir), transform.GetChild(mapNum));
            Instantiate(enemyHPCanvas_Prefab, obj.transform.position + Vector3.up * 2, Quaternion.identity, obj.transform);
            obj.name = type;

            index = 0;
            for (int i = 0; i < transform.GetChild(mapNum).childCount; i++)
            {
                if (transform.GetChild(mapNum).GetChild(i).name.Contains(type) && !transform.GetChild(mapNum).GetChild(i).name.Contains("Target"))
                    index++;
            }
            obj.name = type + index;
        }
        obj.GetComponent<Enemy>().MoveDist = moveDist;

        Enemies.Add(obj);

        if (isBossStage)
        {
            if (!obj.name.Contains("Target"))
                obj.SetActive(false);
        }
           
        return obj.name;
    }

    public string CreateFugitive(int mapNum, string type, Vector3 spawnPos, float playerCognitiveDist, float fugitiveCognitiveDist, float moveDistance, Vector3[] rallyPoints, float moveSpeed = 1, float hp = 20, bool isTargetEnemy = false)
    {
        int index = 0;
        GameObject obj = null;
        mapNum--;

        switch (type)
        {
            case "Jiruru":
                index = 0;
                break;
            case "Bansha":
                index = 1;
                break;
            case "Witched":
                index = 2;
                break;

            default:
                Debug.LogWarning(type);
                break;
        }

        //if (transform.childCount == 0)
        //    ClassifyMap(mapNum);

        if (isTargetEnemy)
        {
            obj = Instantiate(Fugitive_Prefab[1], spawnPos, Quaternion.identity, transform.GetChild(mapNum));
            Instantiate(targetEnemyHPCanvas_Prefab, Vector3.zero, Quaternion.identity, obj.transform);
            obj.name = "Target-" + type;
        }
        else
        {
            obj = Instantiate(Fugitive_Prefab[index], spawnPos, Quaternion.identity, transform.GetChild(mapNum));
            obj.name = type;

            index = 0;
            for (int i = 0; i < transform.GetChild(mapNum).childCount; i++)
            {
                if (transform.GetChild(mapNum).GetChild(i).name.Contains(type) && !transform.GetChild(mapNum).GetChild(i).name.Contains("Target"))
                    index++;
            }
            obj.name = type + index;
        }
        Fugitive fugitive = obj.GetComponent<Fugitive>();
        fugitive.PlayerCognitiveDist = playerCognitiveDist;
        fugitive.FugitiveCognitiveDist = fugitiveCognitiveDist;
        fugitive.MoveDistance = moveDistance;
        fugitive.SetRallyPoints(rallyPoints);
        fugitive.MoveSpeed = moveSpeed;
        fugitive.HP += hp;

        Enemies.Add(obj);

        return obj.name;
    }

    public void DestroyEnemy(string className,string enemyName)
    {
        GameObject obj;
        switch(className)
        {
            case "enemy":
                if (!enemyName.Contains("Target"))
                {
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        if (Enemies[i].name.Equals(enemyName))
                        {
                            obj = Enemies[i];
                            Enemies.RemoveAt(i);
                            Destroy(obj);
                            break;
                        }
                    }
                }
                else
                {
                    // Target을 죽였을 경우
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        obj = Enemies[i];
                        Destroy(obj);
                    }
                    Enemies.Clear();
                    pageMap.SetPageClearData();
                }
                break;

            case "fugitive":
                bool value = true;
                // 실패했을 경우
                if (!enemyName.Contains("Target"))
                    value = false;

                // 모든 적 파괴
                for (int i = 0; i < Enemies.Count; i++)
                {
                    obj = Enemies[i];
                    Destroy(obj);
                }
                Enemies.Clear();
                pageMap.SetPageClearData(value);
                break;

            default:
                Debug.LogWarning(className);
                break;
        }
    }

    public void DeactivateEnemy(string name)
    {
        for(int i=0; i< Enemies.Count; i++)
        {
            if (Enemies[i].name.Equals(name))
            {
                Enemies[i].GetComponent<Enemy>().StopAllCoroutines();
                Enemies[i].SetActive(false);
                    
                break;
            }
        }
    }

    public void ActivateEnemy(string EnemyType)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].name.Contains(EnemyType) && !Enemies[i].activeSelf)
            {
                Enemies[i].SetActive(true);
                break;
            }
        }
    }

    public void SetEnemyAboutCurrPageMap(int mapNum, Page page)
    {
        switch (page.pageType)
        {
            case Page.PageType.BATTLE:
                SetEnemies(mapNum, (BattlePage)page);
                break;

            case Page.PageType.RIDDLE:
                SetFugitives(mapNum, (RiddlePage)page);
                break;

            default:
                break;
        }
    }

    private void SetEnemies(int mapNum, BattlePage page)
    {
        //Debug.Log("적들 생성");
        for (int i = 0; i < page.enemyTypes.Length; i++)
        {
            if(page.PageDataName == "1-11")
                CreateEnemy(mapNum, page.enemyTypes[i], page.enemySpawnPos[i], page.enemyDir[i], page.enemyMoveDist[i], false, true);
            else
                CreateEnemy(mapNum, page.enemyTypes[i], page.enemySpawnPos[i], page.enemyDir[i], page.enemyMoveDist[i]);
        }
           

        CreateEnemy(mapNum, page.targetEnemyType, page.targetEnemySpawnPos, page.targetEnemyDir, page.targetEnemyMoveDist, true);
    }

    private void SetFugitives(int mapNum, RiddlePage page)
    {
        Debug.Log("Fugitive 적 세팅 : "+page.PageDataName);
        for (int i = 0; i < page.enemyTypes.Length; i++) 
            CreateFugitive(mapNum, page.enemyTypes[i], page.enemySpawnPos[i], page.playerCognitiveDist[i], page.fugitiveCognitiveDist[i], page.moveDistance[i], page.rallyPoints);
        Debug.Log(page.enemyTypes.Length);
        Debug.Log($"speed : {page.target_moveSpeed}  hp : {page.target_hp}");
        CreateFugitive(mapNum, page.targetEnemyType, page.targetEnemySpawnPos, page.target_playerCognitiveDist, page.target_fugitiveCognitiveDist, page.target_moveDistance, page.rallyPoints, page.target_moveSpeed, page.target_hp, true);
    }
}
