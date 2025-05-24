using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>, IListener
{
    private int maxEnemyNum;
    private int currEnemyNum;
    private NodeType currNodeType;
    private PhaseData currPhaseData;
    private PlayerState playerState;

    [SerializeField] private bool isFixedMap = false;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] private FixedMap fixedMap;

    public int CurrEnemyNum // 페이즈 끝날시 변경
    {
        get { return currEnemyNum; }
        set
        {
            currEnemyNum = value;

            // 맨 처음 초기화할 때
            if (value > 1) return;

            Debug.Log($"적 개수 : {currEnemyNum}");
            // 모든 페이지 완료시
            if (currEnemyNum <= 0)
            {
                EventManager.Instance.PostNotification(EVENT_TYPE.Stage_Clear, this);
                if(isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
                else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
            }
            playerState.Coin += 100;
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Failed, this);

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
            // 배틀 노드가 아닌 경우
            if(nodeType == currNodeType) return (false, currNodeType);
        }

        return (true, currNodeType);
    }

    public void OnEvent(EVENT_TYPE eventType, UnityEngine.Component Sender, object Param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Stage_Start:
                Node node = (Node)Param;
                currNodeType = node.type;
                SetPaseData(node);
                break;
        }
    }

    private void SetPaseData(Node node)
    {
        switch (node.type)
        {
            case NodeType.Start:
            case NodeType.Battle_Normal:
            case NodeType.Battle_Elite:
            case NodeType.Battle_Elite1:
            case NodeType.Battle_Elite2:
            case NodeType.Boss:
            case NodeType.Unknown:
                SetCurrPhaseData(node);
                SpawnEnemies();
                break;
            case NodeType.Quest:
                SetCurrPhaseData(node);
                break;
        }
    }

    private void SetCurrPhaseData(Node node)
    {
        //ProceduralMapGenerator mapGenerator = GameObject.Find("ProceduralMap").GetComponent<ProceduralMapGenerator>();
        GameObject currMap = node.map;
        Debug.Log($"현재 맵 id : {node.id}");

        currPhaseData = currMap.GetComponentInChildren<PhaseData>();
    }

    public void SpawnEnemies()
    {
        EnemySetter.Instance.SpawnEnemys(currPhaseData.Enemies);
    }
}