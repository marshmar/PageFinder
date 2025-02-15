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
public class GameData : Singleton<GameData>, IListener
{

    private int maxWaveNum;
    private int currWaveNum;
    private int maxEnemyNum;
    private int currEnemyNum;

    PhaseDatas phaseDatas;

    private PageType currPageType;

    public int CurrWaveNum // ������ ������ ����
    {
        get { return currWaveNum; }
        set
        {
            currWaveNum = value;
            Debug.Log($"Wave {currWaveNum}�� ���� : Enemy Spawn");

            // ��� ������ �Ϸ��
            if (currWaveNum > maxWaveNum)
            {
                // ����ȭ������ ����
                EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Reward);
                Debug.Log("���� ������ ��� ������ �Ϸ�!");
            }
            else
            {
                maxWaveNum = phaseDatas.phaseDatas.Count;
                currEnemyNum = phaseDatas.phaseDatas[currWaveNum - 1].enemyDatas.Count;
                EnemySetter.Instance.SpawnEnemys(phaseDatas.phaseDatas[currWaveNum - 1].enemyDatas);

                Debug.Log($"maxWaveNum:{maxWaveNum}     currWaveNum:{currWaveNum}   currEnemyNum:{currEnemyNum}");
            }
        }
    }

    public int CurrEnemyNum // ������ ������ ����
    {
        get { return currEnemyNum; }
        set
        {
            currEnemyNum = value;

            // ��� ������ �Ϸ��
            if (currEnemyNum <= 0)
            {
                CurrWaveNum++;
            }
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.PageMapToBattle, this);
    }


    public void SetCurrPageType(PageType pageType)
    {
        currPageType = pageType;
    }

    public PageType GetCurrPageType()
    {
        return currPageType;
    }

    public void OnEvent(EVENT_TYPE eventType, UnityEngine.Component Sender, object Param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.PageMapToBattle:
                Debug.Log("pageMap -> Battle");
                GameObject mapPrefab = (GameObject)Param;

                phaseDatas = mapPrefab.GetComponentInChildren<PhaseDatas>();
                maxWaveNum = phaseDatas.GetMaxPhaseNum();
                CurrWaveNum = 1;

                // ��Ʋ UI Ȱ��ȭ
                EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
                break;
        }
    }
}