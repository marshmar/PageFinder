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

    public int CurrStageNum // ���� ���� ����
    {
        get { return currStageNum; }
        set 
        {
           
            currStageNum = value;
        }
    }

    public int CurrPageNum // ��Ż Ÿ�� ����
    {
        get { return currPageNum; }
        set { currPageNum = value; }
    }

    public int CurrWaveNum // ������ ������ ����
    {
        get { return currWaveNum; }
        set 
        { 
            currWaveNum = value;
            Debug.Log($"Wave {currWaveNum}�� ���� : Enemy Spawn");
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