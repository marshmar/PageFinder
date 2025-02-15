using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum PageType
{
    BATTLE,
    RIDDLE,
    SHOP,
}
public class GameData : Singleton<GameData>
{
    [SerializeField]
    private int currStageNum;
    [SerializeField]
    private int currPageNum;
    [SerializeField]
    private int currWaveNum;

    private PageType currPageType;

    public int CurrStageNum // 보스 깨면 변경
    {
        get { return currStageNum; }
        set 
        {
           
            currStageNum = value;
        }
    }

    public int CurrPageNum // 포탈 타면 변경
    {
        get { return currPageNum; }
        set { currPageNum = value; }
    }

    public int CurrWaveNum // 페이즈 끝날시 변경
    {
        get { return currWaveNum; }
        set 
        { 
            currWaveNum = value;
            Debug.Log($"Wave {currWaveNum}로 변경 : Enemy Spawn");
        }
    }

    public void SetCurrPageType(PageType pageType)
    {
        currPageType = pageType;
    }

    public PageType GetCurrPageType()
    {
        return currPageType;
    }

}