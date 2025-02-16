using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Enemy;


public class PhaseData : MonoBehaviour
{
    [System.Serializable]
    private class EnemyType
    {
        public Enemy.EnemyType type;
        public List<Vector3> destinations;
    }

    [SerializeField]
    private List<EnemyType> enemyTypes = new List<EnemyType>();

    private List<EnemyData> enemies = new List<EnemyData>(); // 현재 페이즈의 모든 적 정보

    public List<EnemyData> Enemies
    {
        get { return enemies; }
    }

    public void SetEnemyDatas(Vector3 mapPos, int colNum)
    {
        for (int i=0; i< enemyTypes.Count; i++)
        {
            foreach (var pos in enemyTypes[i].destinations)
                Debug.Log(pos);

            // 적 종류에 대한 기본 스탯으로 생성
            EnemyData enemyData = EnemySetter.Instance.SetEnemyData(enemyTypes[i].type, enemyTypes[i].destinations);

            // 열에 대해 스탯 변경
            EnemySetter.Instance.SetEnemyStat(enemyData, colNum, mapPos);

            Debug.Log(enemyData);
            enemies.Add(enemyData);
        }
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

    public List<EnemyData> GetEnemyDatas()
    {
        return enemies;
    }
}
