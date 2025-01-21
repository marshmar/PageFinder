using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePageData : ScriptableObject
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

    public string[] types = { "" };

    public Vector3[] spawnPos = { Vector3.zero };

    public Vector3[] dir = { Vector3.zero };

    public int[] moveDist = { 0 };

    public int[] maxHp;
    public int[] atk;
    public float[] atkSpeed;
}
