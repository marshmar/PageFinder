using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameData : Singleton<GameData>
{
    private List<List<List<List<EnemyData>>>> stageData
        = new List<List<List<List<EnemyData>>>>();

    [SerializeField]
    private int currStageNum;
    [SerializeField]
    private int currPageNum;
    [SerializeField]
    private int currWaveNum;

    public int CurrStageNum // 보스 깨면 변경
    {
        get { return currStageNum; }
        set { currStageNum = value; }
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
            EnemySetter.Instance.SpawnEnemy();
            Debug.Log($"Wave {currWaveNum}로 변경 : Enemy Spawn");
        }
    }

    private void Start()
    {
        EnemyCSVReader.Instance.ReadCSV();
    }

    public void SetStageData(ref EnemyData enemyData, int stageNum, int pageNum, int waveNum)
    {
        List<List<List<EnemyData>>> pageData;

        // 스테이지가 있는 경우
        if (stageNum > 0 && stageNum <= stageData.Count())
            pageData = stageData[stageNum-1];
        else
        {
            pageData = new List<List<List<EnemyData>>>();
            stageData.Add(pageData);
        }
        SetPageDatas(ref enemyData, ref pageData, pageNum, waveNum);
    }

    public void SetPageDatas(ref EnemyData enemyData, ref List<List<List<EnemyData>>> pageData, int pageNum, int waveNum)
    {
        List<List<EnemyData>> waveData;

        // 페이지가 있는 경우
        if (pageNum > 0 && pageNum <= pageData.Count())
            waveData = pageData[pageNum-1];
        else
        {
            waveData = new List<List<EnemyData>>();
            pageData.Add(waveData);
        }

        SetWaveDatas(ref enemyData, ref waveData, waveNum);
    }

    public void SetWaveDatas(ref EnemyData enemyData, ref List<List<EnemyData>> waveData, int waveNum)
    {
        List<EnemyData> enemyDatas;

        // 웨이브가 있는 경우
        if (waveNum > 0 && waveNum <= waveData.Count())
            enemyDatas = waveData[waveNum-1];
        else
        {
            enemyDatas = new List<EnemyData>();
            waveData.Add(enemyDatas);
        }

        enemyDatas.Add(enemyData);
    }

    /// <summary>
    /// 현재 스테이지, 페이지, 웨이브의 적의 정보를 얻는다.
    /// </summary>
    /// <param name="enemyNum">적 번호</param>
    /// <returns></returns>
    public void GetCurrEnemyDatas(ref List<EnemyData> enemyDatas)
    {
        var pageData = stageData[currStageNum-1];
        var waveData = pageData[currPageNum-1];
        
        enemyDatas = waveData[currWaveNum-1];
    }
}