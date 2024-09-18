using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]
    private GameObject[] Enemies_Prefab;

    private List<GameObject> Enemies = new List<GameObject>();


    private void Start()
    {
        ClassifyMap(0);
    }

    public void ClassifyMap(int mapNum)
    {
        GameObject map = new GameObject(mapNum.ToString());
        map.transform.parent = transform;
    }

    /// <summary>
    /// 적을 생성한다.
    /// </summary>
    /// <param name="mapNum">적이 생성되는 맵 번호</param>
    /// <param name="type">적의 종류</param>
    /// <param name="pos">적 생성 위치</param>
    /// <returns>현재 만든 적의 이름</returns>
    public string CreateEnemy(int mapNum, string type, Vector3 pos)
    {
        int index = 0;
        GameObject obj = null;

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

        obj = Instantiate(Enemies_Prefab[index], pos, Quaternion.identity, transform.GetChild(mapNum));

        index = 0;
        for(int i=0; i < transform.GetChild(mapNum).childCount; i++)
        {
            if (transform.GetChild(mapNum).GetChild(i).name.Contains(type))
                index++;
        }
        obj.name = type + index;
        Enemies.Add(obj);

        return obj.name;
    }

    public void DeactivateEnemy(string name)
    {
        for(int i=0; i< Enemies.Count; i++)
        {
            if (Enemies[i].name.Equals(name))
            {
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
}
