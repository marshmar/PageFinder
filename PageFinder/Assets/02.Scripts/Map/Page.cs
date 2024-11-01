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
    [SerializeField]
    protected string pageDataName;
    [SerializeField]
    protected bool isClear;
    [SerializeField]
    protected Vector3 playerSpawnPos;

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

    public string getPageTypeString()
    {
        switch(pageType)
        {
            case PageType.BATTLE:
                return "Battle";

            case PageType.RIDDLE:
                return "RiddleBook";

            case PageType.TRANSACTION:
                return "Transaction";

            case PageType.MIDDLEBOSS:
                return "Battle";

            default:
                Debug.LogWarning(pageType);
                return "";
        }
    }
}
