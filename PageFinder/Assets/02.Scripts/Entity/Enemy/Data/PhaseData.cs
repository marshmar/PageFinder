using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class PhaseData : MonoBehaviour
{
    public int phaseNum;

    [HideInInspector]
    public List<EnemyData> enemyDatas; // 현재 페이즈의 모든 적 정보

    private void Start()
    {
        SetEnemyDatas();
    }

    private void SetEnemyDatas()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Enemy.EnemyType enemyType = GetEnemyType(child.gameObject.name);

            List<Vector3> destinatnios = new List<Vector3>();
            SetDestinations(ref destinatnios, child);

            EnemyData enemyData = EnemySetter.Instance.SetEnemyData(0, enemyType, destinatnios);
            Debug.Log($"{i}번째 적 목적지 개수 : {destinatnios.Count}");
            enemyDatas.Add(enemyData);
        }
    }

    private void SetDestinations(ref List<Vector3> destinatnios, Transform tr)
    {
        for (int i = 0; i < tr.transform.childCount; i++)
            destinatnios.Add(tr.GetChild(i).position);
    }

    private Enemy.EnemyType GetEnemyType(string s)
    {
        switch(s)
        {
            case "Jiruru":
                return Enemy.EnemyType.Jiruru;

            case "Bansha":
                return Enemy.EnemyType.Bansha;

            case "Witched":
                return Enemy.EnemyType.Witched;

            case "Fugitive":
                return Enemy.EnemyType.Fugitive;

            case "Target_Fugitive":
                return Enemy.EnemyType.Target_Fugitive;

            case "Chaser_Jiruru":
                return Enemy.EnemyType.Chaser_Jiruru;

            case "Fire_Jiruru":
                return Enemy.EnemyType.Fire_Jiruru;

            default:
                Debug.LogWarning(s);
                return Enemy.EnemyType.Jiruru;
        }
    }
}
