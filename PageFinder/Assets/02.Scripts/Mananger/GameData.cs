using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>, IListener
{
    private bool isInRound = false;
    private int maxEnemyNum;
    private int currEnemyNum;
    private NodeType currNodeType;
    private PhaseData currPhaseData;
    private PlayerState playerState;
    private List<EnemyData> enemyData;
    private bool gameEnd = false;

    [SerializeField] private bool isFixedMap = false;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] private FixedMap fixedMap;

    public int CurrEnemyNum // ������ ������ ����
    {
        get { return currEnemyNum; }
        set
        {
            currEnemyNum = value;

            // Initialize
            if (value > 1) return;

            // Page Cleared
            if (currEnemyNum <= 0)
            {
                if (gameEnd) return;
                if(isInRound) { SpawnEnemies2Round(); return; }
                AudioManager.Instance.Play(Sound.end, AudioClipType.SequenceSfx);
                EventManager.Instance.PostNotification(EVENT_TYPE.Stage_Clear, this);
                if(isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
                else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
            }
            playerState.Coin += 100;
        }
    }

    private void Start()
    {
        AddListener();
        playerState = GameObject.FindWithTag("PLAYER").GetComponent<PlayerState>();
    }

    private void OnDestroy()
    {
        RemoveListener();
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
            if(nodeType == currNodeType) return (false, currNodeType);
        }

        return (true, currNodeType);
    }

    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Failed, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Player_Dead, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Failed, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Player_Dead, this);
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
            case EVENT_TYPE.Player_Dead:
                gameEnd = true;
                break;
        }
    }

    private void SetPaseData(Node node)
    {
        switch (node.type)
        {
            case NodeType.Start:
                SetCurrPhaseData(node);
                break;
            case NodeType.Battle_Normal:
            case NodeType.Battle_Elite:
            case NodeType.Battle_Elite1:
            case NodeType.Boss:
            case NodeType.Unknown:
                SetCurrPhaseData(node);
                SpawnEnemies();
                break;
            case NodeType.Battle_Elite2:
                SetCurrPhaseData(node);
                SpawnEnemies1Round();
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
        Debug.Log($"���� �� id : {node.id}");

        currPhaseData = currMap.GetComponentInChildren<PhaseData>();
    }

    public void SpawnEnemies()
    {
        EnemySetter.Instance.SpawnEnemys(currPhaseData.Enemies);
    }

    public void SpawnEnemies1Round()
    {
        int total = currPhaseData.Enemies.Count;
        int half = total / 2;

        List<EnemyData> firstHalf = currPhaseData.Enemies.GetRange(0, half);
        EnemySetter.Instance.SpawnEnemys(firstHalf);
        enemyData = currPhaseData.Enemies;
        isInRound = true;
    }

    public void SpawnEnemies2Round()
    {
        int total = enemyData.Count;
        int half = total / 2;

        List<EnemyData> secondHalf = enemyData.GetRange(half, total - half);
        EnemySetter.Instance.SpawnEnemys(secondHalf);
        isInRound = false;
    }
}