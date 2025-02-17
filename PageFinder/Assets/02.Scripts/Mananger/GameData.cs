using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;


public class GameData : Singleton<GameData>, IListener
{
    private int maxEnemyNum;
    private int currEnemyNum;

    private NodeType currNodeType;
    private PhaseData currPhaseData;

    private PlayerState playerState;

    public int CurrEnemyNum // ������ ������ ����
    {
        get { return currEnemyNum; }
        set
        {
            currEnemyNum = value;

            // �� ó�� �ʱ�ȭ�� ��
            if (value > 1)
                return;

            Debug.Log($"�� ���� : {currEnemyNum}");
            // ��� ������ �Ϸ��
            if (currEnemyNum <= 0)
            {
                EnemySetter.Instance.ClearEnemies();
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
    
    public (bool, NodeType) isBattlePage()
    {
        List<NodeType> nodesThatArentBattle = new List<NodeType> { NodeType.Quest, NodeType.Market};

        foreach(NodeType nodeType in nodesThatArentBattle)
        {
            // ��Ʋ ��尡 �ƴ� ���
            if(nodeType == currNodeType)
                return (false, currNodeType);
        }

        return (true, currNodeType);
    }

    public void OnEvent(EVENT_TYPE eventType, UnityEngine.Component Sender, object Param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.PageMapUIToGamePlay:
                Node node = (Node)Param;
                Debug.Log($"PageMap UI-> GamePlay");

                currNodeType = node.type;
                
                switch (node.type)
                {
                    // ��Ʋ
                    case NodeType.Start:
                    case NodeType.Battle_Normal:
                    case NodeType.Battle_Elite:
                    case NodeType.Boss:

                    case NodeType.Treasure:
                    case NodeType.Comma:
                    case NodeType.Unknown:
                        SetCurrPhaseData(node);
                        SpawnEnemies();
                        // ��Ʋ UI Ȱ��ȭ
                        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
                        break;

                    case NodeType.Quest:
                        // ���� ������ ������ �����ϰ� ����Ʈ UI Ȱ��ȭ -> å �� �а� �������� UI ��Ȱ��ȭ�� �׶� ���� ����
                        SetCurrPhaseData(node);
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

    private void SetCurrPhaseData(Node node)
    {
        ProceduralMapGenerator mapGenerator = GameObject.Find("ProceduralMap").GetComponent<ProceduralMapGenerator>();
        GameObject currMap = node.map;
        Debug.Log($"���� �� id : {node.id}");

        currPhaseData = currMap.GetComponentInChildren<PhaseData>();
    }

    public void SpawnEnemies()
    {
        EnemySetter.Instance.SpawnEnemys(currPhaseData.Enemies);
    }
}