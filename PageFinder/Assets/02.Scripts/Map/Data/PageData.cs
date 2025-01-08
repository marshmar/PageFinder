using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageData : ScriptableObject
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
}
