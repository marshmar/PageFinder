using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class GameData : Singleton<GameData>, IListener
{
    private int maxEnemyNum;
    private int currEnemyNum;

    private NodeType currNodeType;
    private PhaseData currPhaseData;

    private PlayerState playerState;

    public int CurrEnemyNum // 페이즈 끝날시 변경
    {
        get { return currEnemyNum; }
        set
        {
            currEnemyNum = value;
            Debug.Log($"적 개수 : {currEnemyNum}");
            // 모든 페이지 완료시
            if (currEnemyNum <= 0)
            {
                EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Reward);
            }
            playerState.Coin += 100;
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.PageMapUIToGamePlay, this);
        playerState = GameObject.FindWithTag("PLAYER").GetComponent<PlayerState>();
    }


    public void SetCurrPageType(NodeType pageType)
    {
        currNodeType = pageType;
    }

    public NodeType GetCurrPageType()
    {
        return currNodeType;
    }
    
    public bool isBattlePage()
    {
        if (currNodeType == NodeType.Start || currNodeType == NodeType.Battle_Elite
            || currNodeType == NodeType.Battle_Normal || currNodeType == NodeType.Boss)
        {
            return true;
        }
        return false;
    }

    public void OnEvent(EVENT_TYPE eventType, UnityEngine.Component Sender, object Param)
    {
        ProceduralMapGenerator mapGenerator = null;
        GameObject currMap = null;
        PhaseData phaseData = null;

        switch (eventType)
        {
            case EVENT_TYPE.PageMapUIToGamePlay:
                Node node = (Node)Param;
                Debug.Log($"PageMap UI-> GamePlay");

                currNodeType = node.type;
                
                switch (node.type)
                {
                    // 배틀
                    case NodeType.Start:
                    case NodeType.Battle_Normal:
                    case NodeType.Battle_Elite:
                    case NodeType.Boss:

                    case NodeType.Treasure:
                    case NodeType.Comma:
                    case NodeType.Unknown:
                        mapGenerator = GameObject.Find("ProceduralMap").GetComponent<ProceduralMapGenerator>();
                        currMap = node.map;
                        Debug.Log($"현재 맵 id : {node.id}");

                        // 모든 적 스폰
                        phaseData = currMap.GetComponentInChildren<PhaseData>();
                        Debug.Log(phaseData.Enemies);
                        EnemySetter.Instance.SpawnEnemys(phaseData.Enemies);
                        currEnemyNum = phaseData.Enemies.Count;
                        Debug.Log($"적 개수 : {currEnemyNum}");

                        // 배틀 UI 활성화
                        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
                        break;

                    case NodeType.Quest:
                        mapGenerator = GameObject.Find("ProceduralMap").GetComponent<ProceduralMapGenerator>();
                        currMap = node.map;

                        // 모든 적 스폰
                        phaseData = currMap.GetComponentInChildren<PhaseData>();
                        EnemySetter.Instance.SpawnEnemys(phaseData.Enemies);
                        currEnemyNum = phaseData.Enemies.Count;
                        Debug.Log($"적 개수 : {currEnemyNum}");

                        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.RiddleBook);
                        break;

                    case NodeType.Market:
                        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Shop);
                        break;

                    default:
                        Debug.LogWarning(node.type);
                        break;
                }
                break;
        }
    }
}