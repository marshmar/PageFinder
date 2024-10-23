using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Page
{
    public enum PageType
    {
        BATTLE,
        TRANSACTION,
        RIDDLE,
        MIDDLEBOSS
    }

    public PageType pageType;
    public string pageDataName;
    public bool isClear;
    public Vector3 playerSpawnPos;

    public string[] enemyTypes = { ""};
    public Vector3[] enemySpawnPos = { Vector3.zero};
    public Vector3[] enemyDir = { Vector3.zero };
    public int[] enemyMoveDist = { 0 };

    public string targetEnemyType;
    public Vector3 targetEnemySpawnPos;
    public Vector3 targetEnemyDir = Vector3.zero;
    public int targetEnemyMoveDist = 0;


    public string PageDataName
    {
        get
        {
            return pageDataName;
        }
        set
        {
            pageDataName = value;
        }
    }

    public bool IsClear
    {
        get
        {
            return isClear;
        }
        set
        {
            isClear = value;
        }
    }


    public Vector3 GetSpawnPos()
    {
        return playerSpawnPos;
    }
}
