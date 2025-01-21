using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private string currTargetName;

    PageMap pageMap;

    public string CurrTargetName
    {
        get
        {
            return currTargetName;
        }
        set
        {
            currTargetName = value;
        }
    }


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
    public string CreateEnemy(int mapNum, BattlePage page, int index, bool isBossStage = false)
    {
        int typeIndex = GetPrefabIndex(page.types[index]);
        GameObject obj = null;

        obj = Instantiate(Enemies_Prefab[typeIndex], page.spawnPos[index], Quaternion.Euler(page.dir[index]), transform.GetChild(mapNum));

        // 타겟인 경우
        if (index == 0)
        {
            GameObject targetCanvas = Instantiate(targetEnemyHPCanvas_Prefab, Vector3.zero, Quaternion.identity, obj.transform);
            if (!page.types[index].Equals("Witched"))
                obj.transform.localScale = Vector3.one * 1.5f;
            targetCanvas.transform.GetChild(0).GetComponent<TMP_Text>().text = GetEnemyKRType(page.types[index]);
            obj.name = "Target-" + page.types[index];
            currTargetName = GetEnemyKRType(page.types[index]);
        }
        else
        {
            Instantiate(enemyHPCanvas_Prefab, obj.transform.position + Vector3.up * 2, Quaternion.identity, obj.transform);
            obj.name = page.types[index];
        }
        obj.GetComponent<Enemy>().SetStatus(page, index);

        Enemies.Add(obj);

        if (isBossStage)
        {
            if (index != 0)
                obj.SetActive(false);
        }
           
        return obj.name;
    }

    string GetEnemyKRType(string type)
    {
        switch(type)
        {
            case "Jiruru":
                return "지루루";

            case "Bansha":
                return "밴샤";

            case "Witched":
                return "위치드";
            default:
                Debug.LogWarning(type);
                return "";
        }
    }

    int GetPrefabIndex(string type)
    {
        switch (type)
        {
            case "Jiruru":
                return 0;
            case "Bansha":
                return 1;
            case "Witched":
                return 2;
            default:
                Debug.LogWarning(type);
                return 0;
        }
    }


    public string CreateFugitive(int mapNum, RiddlePage page, int index)
    {
        int typeIndex = GetPrefabIndex(page.types[index]);
        GameObject obj = null;

        // 타겟인 경우
        if (index == 0)
        {
            obj = Instantiate(Fugitive_Prefab[1], page.spawnPos[index], Quaternion.identity, transform.GetChild(mapNum));
            //GameObject targetCanvas = Instantiate(targetEnemyHPCanvas_Prefab, Vector3.zero, Quaternion.identity, obj.transform);
            //targetCanvas.transform.GetChild(0).GetComponent<TMP_Text>().text = GetEnemyKRType(page.types[index]);
            obj.name = "Target-" + page.types[index];
            currTargetName = GetEnemyKRType(page.types[index]);
        }
        else
        {
            obj = Instantiate(Fugitive_Prefab[typeIndex], page.spawnPos[index], Quaternion.identity, transform.GetChild(mapNum));
            obj.name = page.types[index];
        }
        obj.GetComponent<Fugitive>().SetStatus(page, index);

        //Debug.Log("변경 후 " + obj.GetComponent<Fugitive>().name + ":" + obj.GetComponent<Fugitive>().HP);

        Enemies.Add(obj);

        return obj.name;
    }

    public void DestroyEnemy(string className, GameObject enemyObj)
    {
        bool isTarget = enemyObj.name.Contains("Target");

        switch (className)
        {
            case "enemy":
                if (!isTarget)
                {
                    Enemies.Remove(enemyObj);
                    Destroy(enemyObj);
                }
                else
                {
                    // Target을 죽였을 경우
                    for (int i = 0; i < Enemies.Count; i++)
                        Destroy(Enemies[i]);

                    Enemies.Clear();
                    pageMap.SetPageClearData();
                }
                break;

            case "fugitive":
                bool value = true;
                // 실패했을 경우
                if (!isTarget)
                    value = false;

                // 모든 적 파괴
                for (int i = 0; i < Enemies.Count; i++)
                    Destroy(Enemies[i]);

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
            case Page.PageType.MIDDLEBOSS:
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
        for (int i = 0; i < page.types.Length; i++)
        {
            if(page.PageDataName == "0_10")
                CreateEnemy(mapNum, page, i, true);
            else
                CreateEnemy(mapNum, page, i);
        }
    }

    private void SetFugitives(int mapNum, RiddlePage page)
    {
        for (int i = 0; i < page.types.Length; i++) 
            CreateFugitive(mapNum, page, i);
    }
}
