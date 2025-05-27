using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FixedMap : MonoBehaviour
{
    [SerializeField] private float nodeSpacing = 3.0f;

    [Header("UI Setting")]
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject battleNormalUI;
    [SerializeField] private GameObject battleEliteUI;
    [SerializeField] private GameObject questUI;
    [SerializeField] private GameObject treasureUI;
    [SerializeField] private GameObject marketUI;
    [SerializeField] private GameObject commaUI;
    [SerializeField] private GameObject bossUI;
    [SerializeField] private GameObject lineUI;

    [Space(10f)]
    [SerializeField] private GameObject battleNormalPastUI;
    [SerializeField] private GameObject battleElitePastUI;
    [SerializeField] private GameObject questPastUI;
    [SerializeField] private GameObject treasurePastUI;
    [SerializeField] private GameObject marketPastUI;
    [SerializeField] private GameObject commaPastUI;
    [SerializeField] private Sprite linePastSprite;

    [Space(10f)]
    [SerializeField] private GameObject battleNormalFutureUI;
    [SerializeField] private GameObject battleEliteFutureUI;
    [SerializeField] private GameObject questFutureUI;
    [SerializeField] private GameObject treasureFutureUI;
    [SerializeField] private GameObject marketFutureUI;
    [SerializeField] private GameObject commaFutureUI;
    [SerializeField] private GameObject bossFutureUI;
    [SerializeField] private Sprite lineFutureSprite;

    [Header("Map Setting")]
    [SerializeField] private GameObject startMap;
    [SerializeField] private GameObject battleNormalMap;
    [SerializeField] private GameObject[] battleEliteMap;
    [SerializeField] private GameObject questMap;
    [SerializeField] private GameObject treasureMap;
    [SerializeField] private GameObject marketMap;
    [SerializeField] private GameObject commaMap;
    [SerializeField] private GameObject bossMap;
    [SerializeField] private GameObject portalPrefab;

    private Node startNode;
    private Node battleNormalNode;
    private Node battleEliteNode1;
    private Node battleEliteNode2;
    private Node questNode;
    private Node treasureNode;
    private Node commaNode;
    private Node bossNode;
    private Node[,] nodes;
    private Camera mainCamera;
    private List<Edge> edges = new();
    private Dictionary<Node, GameObject> nodeUIMap = new(); // Mapping with Node and UI
    private Dictionary<Node, GameObject> worldMapInstances = new(); // Mapping with Node and Map Instance
    private Dictionary<NodeType, GameObject> nodeTypeUIMap;
    private Dictionary<NodeType, GameObject> nodeTypePastUIMap;
    private Dictionary<NodeType, GameObject> nodeTypeFutureUIMap;
    private Dictionary<NodeType, GameObject> nodeTypeWorldMap;

    public Node playerNode { get; private set; }

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.SetPositionAndRotation(new Vector3(0, 3, -6.5f), Quaternion.Euler(Vector3.zero));

        nodeTypeUIMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Start, battleNormalUI },
            { NodeType.Battle_Normal, battleNormalUI },
            { NodeType.Battle_Elite1, battleEliteUI },
            { NodeType.Battle_Elite2, battleEliteUI },
            { NodeType.Quest, questUI },
            { NodeType.Treasure, treasureUI },
            { NodeType.Market, marketUI },
            { NodeType.Comma, commaUI },
            { NodeType.Boss, bossUI }
        };

        nodeTypePastUIMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Start, battleNormalPastUI },
            { NodeType.Battle_Normal, battleNormalPastUI },
            { NodeType.Battle_Elite1, battleElitePastUI },
            { NodeType.Battle_Elite2, battleElitePastUI },
            { NodeType.Quest, questPastUI },
            { NodeType.Treasure, treasurePastUI },
            { NodeType.Market, marketPastUI },
            { NodeType.Comma, commaPastUI }
        };

        nodeTypeFutureUIMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Start, battleNormalFutureUI },
            { NodeType.Battle_Normal, battleNormalFutureUI },
            { NodeType.Battle_Elite1, battleEliteFutureUI },
            { NodeType.Battle_Elite2, battleEliteFutureUI },
            { NodeType.Quest, questFutureUI },
            { NodeType.Treasure, treasureFutureUI },
            { NodeType.Market, marketFutureUI },
            { NodeType.Comma, commaFutureUI },
            { NodeType.Boss, bossFutureUI },
        };

        nodeTypeWorldMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Start, startMap },
            { NodeType.Battle_Normal, battleNormalMap },
            { NodeType.Battle_Elite1, battleEliteMap[0] },
            { NodeType.Battle_Elite2, battleEliteMap[1] },
            { NodeType.Quest, questMap },
            { NodeType.Treasure, treasureMap },
            { NodeType.Market, marketMap },
            { NodeType.Comma, commaMap },
            { NodeType.Boss, bossMap }
        };
        GenerateNodes();
    }

    void GenerateNodes()
    {
        nodes = new Node[8, 1];

        // Create a start node before column 1
        startNode = new(0, 1, new Vector2(-nodeSpacing, 3), NodeType.Start, nodeTypeWorldMap[NodeType.Start]);
        battleNormalNode = new(1, 1, new Vector2(0, 3), NodeType.Battle_Normal, nodeTypeWorldMap[NodeType.Battle_Normal]);
        treasureNode = new(2, 1, new Vector2(nodeSpacing, 3), NodeType.Treasure, nodeTypeWorldMap[NodeType.Treasure]);
        battleEliteNode1 = new(3, 1, new Vector2(2 * nodeSpacing, 3), NodeType.Battle_Elite1, nodeTypeWorldMap[NodeType.Battle_Elite1]);
        questNode = new(4, 1, new Vector2(3 * nodeSpacing, 3), NodeType.Quest, nodeTypeWorldMap[NodeType.Quest]);
        battleEliteNode2 = new(5, 1, new Vector2(4 * nodeSpacing, 3), NodeType.Battle_Elite2, nodeTypeWorldMap[NodeType.Battle_Elite2]);
        commaNode = new(6, 1, new Vector2(5 * nodeSpacing, 3), NodeType.Comma, nodeTypeWorldMap[NodeType.Comma]);
        bossNode = new(7, 1 + 1, new Vector2(6 * nodeSpacing, 3), NodeType.Boss, nodeTypeWorldMap[NodeType.Boss]);

        NodeManager.Instance.AddNode(startNode);
        NodeManager.Instance.AddNode(battleNormalNode);
        NodeManager.Instance.AddNode(treasureNode);
        NodeManager.Instance.AddNode(battleEliteNode1);
        NodeManager.Instance.AddNode(questNode);
        NodeManager.Instance.AddNode(battleEliteNode2);
        NodeManager.Instance.AddNode(commaNode);
        NodeManager.Instance.AddNode(bossNode);

        nodes[0, 1 / 2] = startNode;
        nodes[1, 1 / 2] = battleNormalNode;
        nodes[2, 1 / 2] = treasureNode;
        nodes[3, 1 / 2] = battleEliteNode1;
        nodes[4, 1 / 2] = questNode;
        nodes[5, 1 / 2] = battleEliteNode2;
        nodes[6, 1 / 2] = commaNode;
        nodes[7, 1 / 2] = bossNode;

        CreateNodeUI(startNode);
        CreateNodeUI(battleNormalNode);
        CreateNodeUI(treasureNode);
        CreateNodeUI(battleEliteNode1);
        CreateNodeUI(questNode);
        CreateNodeUI(battleEliteNode2);
        CreateNodeUI(commaNode);
        CreateNodeUI(bossNode);

        ConnectNodes();
    }

    void CreateNodeUI(Node node)
    {
        if (!nodeTypeUIMap.TryGetValue(node.type, out GameObject selectedPrefab) || selectedPrefab == null)
        {
            Debug.LogWarning($"No prefab found for NodeType: {node.type}");
            return;
        }

        // Layout UI by converting node positions to Screen Space
        Vector3 worldPosition = new(node.position.x, node.position.y, 0f);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        GameObject uiElement = Instantiate(selectedPrefab, scrollView.content);

        uiElement.GetComponent<RectTransform>().position = screenPosition;
        uiElement.GetComponent<Button>().onClick.AddListener(() => {
            Portal.Teleport(node.map.transform.GetChild(0).position);
            EventManager.Instance.PostNotification(EVENT_TYPE.Stage_Start, this, node);
            //EventManager.Instance.PostNotification(EVENT_TYPE.PageMapUIToGamePlay, this, node);

            // Enable player location display and next map movement
            NodeManager.Instance.ChangeNodeUI(nodeTypePastUIMap[playerNode.type], playerNode.id);
            if (playerNode.neighborIDs[0] != node.id)
            {
                NodeManager.Instance.GetNodeByID(playerNode.neighborIDs[0]).ui.GetComponent<Button>().enabled = false;
                NodeManager.Instance.GetNodeByID(playerNode.neighborIDs[0]).ui.GetComponent<Animator>().SetBool("isSelectable", false);
                NodeManager.Instance.ChangeNodeUI(nodeTypeUIMap[NodeManager.Instance.GetNodeByID(playerNode.neighborIDs[0]).type], playerNode.neighborIDs[0]);
                foreach (var edge in edges)
                {
                    if (edge.nodeB.id == playerNode.neighborIDs[0]) edge.LineUI.GetComponent<Image>().sprite = lineUI.GetComponent<Image>().sprite;
                }
            }

            foreach (var edge in edges)
            {
                if (edge.nodeA == playerNode && edge.nodeB.id == node.id) edge.LineUI.GetComponent<Image>().sprite = linePastSprite;
            }
            node.ui.GetComponent<Animator>().SetBool("isSelectable", false);
            playerNode.ui.GetComponent<Animator>().SetBool("isPlayerUI", false);
            playerNode = node;
            NodeManager.Instance.ChangeNodeUI(playerUI, playerNode.id);
            if (playerNode.neighborIDs.Count > 0)
            {
                NodeManager.Instance.GetNodeByID(playerNode.neighborIDs[0]).ui.GetComponent<Button>().enabled = true;
            }
            scrollView.transform.parent.gameObject.SetActive(false);
        });

        uiElement.GetComponent<Button>().enabled = false;

        nodeUIMap[node] = uiElement;
        node.ui = uiElement;
    }

    async void ConnectNodes()
    {
        for (int x = 0; x < 7; x++)
        {
            nodes[x, 1 / 2].neighborIDs.Add(nodes[x+1, 1 / 2].id);
            nodes[x+1, 1 / 2].prevNode = nodes[x, 1 / 2];
            edges.Add(new Edge(nodes[x, 1 / 2], nodes[x + 1, 1 / 2], Vector2.Distance(nodes[x, 1 / 2].position, nodes[x + 1, 1 / 2].position)));

            CreateNodeWorldMap(nodes[x, 1 / 2]);
            if (x == 6) CreateNodeWorldMap(nodes[x + 1, 1 / 2]);
        }

        // Enable player location display and next map movement
        NodeManager.Instance.ChangeNodeUI(playerUI, startNode.id);
        NodeManager.Instance.GetNodeByID(startNode.neighborIDs[0]).ui.GetComponent<Button>().enabled = true;

        playerNode = startNode;

        await DrawPaths();

        // ToDo: UI Changed;
        EventManager.Instance.PostNotification(EVENT_TYPE.Stage_Start, this, NodeManager.Instance.GetNodeByID(startNode.id));
        //EventManager.Instance.PostNotification(EVENT_TYPE.PageMapUIToGamePlay, this, NodeManager.Instance.GetNodeByID(startNode.id));

        if (!Utils.IsGraphConnected(startNode, nodes)) Debug.LogError("Graph disconnected");
    }

    void CreateNodeWorldMap(Node node)
    {
        // Convert screen coordinates of a UI node to world coordinates
        if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
        {
            Vector3 screenPosition = uiElement.GetComponent<RectTransform>().position;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

            // Create map prefab at node location and apply spacing
            Vector3 adjustedPosition = new Vector3(worldPosition.x, 0f, 0f) + new Vector3(node.row * 200, 0, node.column * 200);
            GameObject mapInstance = Instantiate(node.map, adjustedPosition, node.map.transform.rotation, this.transform);
            worldMapInstances[node] = mapInstance;
            node.map.GetComponent<Map>().position = adjustedPosition;
            node.map = mapInstance;

            PhaseData phaseData = mapInstance.GetComponentInChildren<PhaseData>();
            if (phaseData) phaseData.SetEnemyDatas(adjustedPosition, node.column);

            if (node == startNode)
            {
                GameObject player = GameObject.FindGameObjectWithTag("PLAYER");
                if (player is not null) player.transform.position = mapInstance.transform.GetChild(0).position;
            }

            CreatePortal(node);
        }
    }

    void CreatePortal(Node currentNode)
    {
        // Check if there is a current node map prefab
        if (worldMapInstances.TryGetValue(currentNode, out GameObject currentMap))
        {
            // Set portal location inside the map
            Vector3 portalPosition = currentMap.transform.GetChild(1).position + new Vector3(0, 0.1f, 0);

            // Create a portal and place it inside the map prefab
            GameObject portal = Instantiate(portalPrefab, portalPosition, Quaternion.Euler(new Vector3(90, 0, 0)), currentMap.transform);
            currentNode.portal = portal.GetComponent<Portal>();

            if (currentNode == startNode) Portal.OnPortalEnter += ActivateNextPage;
        }
    }

    async Task DrawPaths()
    {
        foreach (var edge in edges)
        {
            if (!nodeUIMap.ContainsKey(edge.nodeB)) continue;

            // World coordinates ¡æ screen coordinates conversion
            Vector3 screenPositionA = mainCamera.WorldToScreenPoint(new Vector3(edge.nodeA.position.x + 0.4f, edge.nodeA.position.y, 0f));
            Vector3 screenPositionB = mainCamera.WorldToScreenPoint(new Vector3(edge.nodeB.position.x - 0.4f, edge.nodeB.position.y, 0f));

            edge.LineUI = CreateLineUI(screenPositionA, screenPositionB);
        }

        mainCamera.transform.position = new Vector3(4, 10, -4);
        mainCamera.transform.Rotate(new Vector3(50, 0, 0));
        scrollView.transform.parent.gameObject.SetActive(false);
        await Task.Delay(5000);
    }

    GameObject CreateLineUI(Vector3 start, Vector3 end)
    {
        GameObject lineObject = Instantiate(lineUI, scrollView.content);
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();

        // Set the center position of the line
        rectTransform.position = (start + end) / 2;

        // Set line thickness and length
        float distance = Vector3.Distance(start, end);
        rectTransform.sizeDelta = new Vector2(distance, 20);

        // Set line rotation
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        return lineObject;
    }

    void ActivateNextPage(Portal portal)
    {
        playerNode.ui.GetComponent<Button>().enabled = false;
        foreach (var edge in edges)
        {
            if (edge.nodeA == playerNode) edge.LineUI.GetComponent<Image>().sprite = lineFutureSprite;
        }
        EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.PageMap);
        //scrollView.transform.parent.gameObject.SetActive(true);
        foreach (var nodeID in playerNode.neighborIDs)
        {
            NodeManager.Instance.ChangeNodeUI(nodeTypeFutureUIMap[NodeManager.Instance.GetNodeByID(nodeID).type], nodeID);
            NodeManager.Instance.GetNodeByID(nodeID).ui.GetComponent<Animator>().SetBool("isSelectable", true);
        }
        playerNode.ui.GetComponent<Animator>().SetBool("isPlayerUI", true);
    }

    private void OnDestroy()
    {
        Portal.OnPortalEnter -= ActivateNextPage;
    }
}