using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralMapGenerator : MonoBehaviour
{
    [Header("UI Generation Setting")]
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 10;
    [SerializeField] private float nodeSpacing = 3.0f;
    [SerializeField] private float offset = 0.08f;

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
    [SerializeField] private GameObject battleEliteMap;
    [SerializeField] private GameObject questMap;
    [SerializeField] private GameObject treasureMap;
    [SerializeField] private GameObject marketMap;
    [SerializeField] private GameObject commaMap;
    [SerializeField] private GameObject bossMap;
    [SerializeField] private GameObject portalPrefab;

    private Node startNode;
    private Node[,] nodes;
    private Camera mainCamera;
    private List<Edge> edges = new();
    private Dictionary<Node, GameObject> nodeUIMap = new(); // Mapping with Node and UI
    private Dictionary<Node, GameObject> worldMapInstances = new(); // Mapping with Node and Map Instance
    private Dictionary<NodeType, GameObject> nodeTypeUIMap;
    private Dictionary<NodeType, GameObject> nodeTypePastUIMap;
    private Dictionary<NodeType, GameObject> nodeTypeFutureUIMap;
    private Dictionary<NodeType, GameObject> nodeTypeWorldMap;

    private const int TreasureColumn = 4;
    private const int CommaColumn = 9;
    private int nodeCount = 0;
    private float SingleConnectionProbability = 0.25f;

    public Node playerNode {get; private set;}

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.SetPositionAndRotation(new Vector3(0, 3, -6.5f), Quaternion.Euler(Vector3.zero));

        nodeTypeUIMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Start, battleNormalUI },
            { NodeType.Battle_Normal, battleNormalUI },
            { NodeType.Battle_Elite, battleEliteUI },
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
            { NodeType.Battle_Elite, battleElitePastUI },
            { NodeType.Quest, questPastUI },
            { NodeType.Treasure, treasurePastUI },
            { NodeType.Market, marketPastUI },
            { NodeType.Comma, commaPastUI }
        };

        nodeTypeFutureUIMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Start, battleNormalFutureUI },
            { NodeType.Battle_Normal, battleNormalFutureUI },
            { NodeType.Battle_Elite, battleEliteFutureUI },
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
            { NodeType.Battle_Elite, battleEliteMap },
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
        nodes = new Node[columns, rows];

        // Create a start node before column 1
        startNode = new(-1, rows / 2, new Vector2(-nodeSpacing, rows/2), NodeType.Start, nodeTypeWorldMap[NodeType.Start]);
        NodeManager.Instance.AddNode(startNode);
        CreateNodeUI(startNode);

        // Create nodes in columns 1 to 10
        List<Node> firstColumnNodes = new();

        for (int x = 0; x < columns; x++)
        {
            if(x == TreasureColumn)
            {
                Node treasureNode = new(x, rows / 2, new Vector2(x * nodeSpacing, rows / 2), NodeType.Treasure, nodeTypeWorldMap[NodeType.Treasure]);
                NodeManager.Instance.AddNode(treasureNode);
                nodes[x, rows / 2] = treasureNode;
                CreateNodeUI(treasureNode);               
                continue;
            }
            if(x == CommaColumn)
            {
                Node commaNode = new(x, rows / 2, new Vector2(x * nodeSpacing, rows / 2), NodeType.Comma, nodeTypeWorldMap[NodeType.Comma]);
                NodeManager.Instance.AddNode(commaNode);
                nodes[x, rows / 2] = commaNode;
                CreateNodeUI(commaNode);
                break;
            }

            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new(x * nodeSpacing * Random.Range(0.98f, 1.02f), y * Random.Range(1 - offset, 1 + offset));

                Node newNode = new(x, y, position, NodeType.Unknown, nodeTypeWorldMap[NodeType.Start]);
                NodeManager.Instance.AddNode(newNode);
                nodes[x, y] = newNode;

                if (x == 0) firstColumnNodes.Add(newNode);
            }
        }

        // Create a boss node after column 10
        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows/2), NodeType.Boss, nodeTypeWorldMap[NodeType.Boss]);
        NodeManager.Instance.AddNode(bossNode);
        CreateNodeUI(bossNode);

        HandleFirstColumnNodes(startNode, firstColumnNodes);
        ConnectNodes(bossNode);
    }

    void ConnectNodes(Node bossNode)
    {
        var activeNodes = InitializeActiveNodes();

        for (int x = 0; x < columns - 1; x++)
        {
            var nextActiveNodes = new HashSet<Node>();
            var hasTwoNeighbors = new bool[2];
            if (x == TreasureColumn)
            {
                nodeCount = 0;
                SingleConnectionProbability = 0.25f;
            }

            // Connecting each node to its neighbors
            foreach (var currentNode in activeNodes)
            {
                var neighborCandidates = GetNeighborCandidates(currentNode, x);
                if (x != TreasureColumn && x != CommaColumn) SetNodeTypeAndUI(currentNode, x);

                Vector2 currentPos = currentNode.position;

                // Random Neighbor Connection
                while (neighborCandidates.Count > 0)
                {
                    Node nextNode = neighborCandidates[Random.Range(0, neighborCandidates.Count)];
                    neighborCandidates.Remove(nextNode);

                    if (nextActiveNodes.Count == 5 && nextNode.prevNode == null) continue;
                    if (IsCrossingAnyEdge(currentPos, nextNode.position)) continue;

                    // Neighborhood Candidate Connection
                    currentNode.neighborIDs.Add(nextNode.id);
                    nextNode.prevNode = currentNode;
                    edges.Add(new Edge(currentNode, nextNode, Vector2.Distance(currentPos, nextNode.position) * Random.Range(1 - offset, 1 + offset)));
                    nextActiveNodes.Add(nextNode);
                    nodeCount++;

                    if (nodeCount == 8) SingleConnectionProbability = 1f;

                    if (Random.value < SingleConnectionProbability || x == 3 || x == 8) break; // Single Connection
                    else // Multiple Connection
                    {
                        if (currentNode.neighborIDs.Count >= 2 && x != 4) break; // When there are two connected neighboring nodes
                        else if (hasTwoNeighbors.Contains(false)) // When dual connection is possible
                        {
                            if (!hasTwoNeighbors[0]) hasTwoNeighbors[0] = true;
                            else hasTwoNeighbors[1] = true;
                            continue;
                        }
                        else if (x == 4 && neighborCandidates.Count > 1) continue; // Connections from Treasure Node
                        else break;
                    }
                }
                CreateNodeWorldMap(currentNode);
            }

            CleanUpUnusedNodes(x + 1, nextActiveNodes);
            activeNodes = nextActiveNodes;
        }

        HandleFinalBossNode(bossNode);
    }

    HashSet<Node> InitializeActiveNodes()
    {
        var active = new HashSet<Node>();
        // Add the node in the first column to the active node
        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null) active.Add(nodes[0, y]);
        }
        return active;
    }

    List<Node> GetNeighborCandidates(Node currentNode, int x)
    {
        var candidates = new List<Node>();

        if (x == 3 || x == 8)
        {
            candidates.Add(nodes[x + 1, rows / 2]);
        }
        else if (x == 4)
        {
            for (int y = 0; y < rows; y++)
            {
                candidates.Add(nodes[x + 1, y]);
            }
        }
        else
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                int nextY = currentNode.row + offsetY;
                if (nextY >= 0 && nextY < rows && nodes[x + 1, nextY] != null)
                {
                    candidates.Add(nodes[x + 1, nextY]);
                }
            }
        }

        return candidates;
    }

    void SetNodeTypeAndUI(Node node, int x)
    {
        node.type = DetermineNodeType(x, node.row);
        node.map = nodeTypeWorldMap[node.type];
        CreateNodeUI(node);
    }

    bool IsCrossingAnyEdge(Vector2 start, Vector2 end)
    {
        foreach (var edge in edges)
        {
            if (Utils.IsCrossing(start, end, edge.nodeA.position, edge.nodeB.position)) return true;
        }
        return false;
    }

    void CleanUpUnusedNodes(int x, HashSet<Node> activeNodes)
    {
        for (int y = 0; y < rows; y++)
        {
            Node node = nodes[x, y];
            if (node != null && !activeNodes.Contains(node))
            {
                nodes[x, y] = null;
                NodeManager.Instance.RemoveNode(node);

                if (nodeUIMap.TryGetValue(node, out var uiElement))
                {
                    Destroy(uiElement);
                    nodeUIMap.Remove(node);
                }
            }
        }
    }

    void CreateNodeWorldMap(Node node)
    {
        // Convert screen coordinates of a UI node to world coordinates
        if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
        {
            Vector3 screenPosition = uiElement.GetComponent<RectTransform>().position;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

            // Create map prefab at node location and apply spacing
            Vector3 adjustedPosition = new Vector3(worldPosition.x, 0f, 0f) + new Vector3(node.row* 100, 0, node.column * 100);
            GameObject mapInstance = Instantiate(node.map, adjustedPosition, node.map.transform.rotation, this.transform);
            worldMapInstances[node] = mapInstance;
            node.map.GetComponent<Map>().position = adjustedPosition;
            node.map = mapInstance;

            PhaseData phaseData = mapInstance.GetComponentInChildren<PhaseData>();
            if (phaseData) phaseData.SetEnemyDatas(adjustedPosition, node.column);

            if (node == startNode)
            {
                GameObject player = GameObject.FindGameObjectWithTag("PLAYER");
                if(player is not null) player.transform.position = mapInstance.transform.GetChild(0).position;
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
            foreach(int nodeID in playerNode.neighborIDs)
            {
                if (nodeID != node.id)
                {
                    NodeManager.Instance.GetNodeByID(nodeID).ui.GetComponent<Button>().enabled = false;
                    NodeManager.Instance.GetNodeByID(nodeID).ui.GetComponent<Animator>().SetBool("isSelectable", false);
                    NodeManager.Instance.ChangeNodeUI(nodeTypeUIMap[NodeManager.Instance.GetNodeByID(nodeID).type], nodeID);
                    foreach (var edge in edges)
                    {
                        if (edge.nodeB.id == nodeID) edge.LineUI.GetComponent<Image>().sprite = lineUI.GetComponent<Image>().sprite;
                    }
                }
            }

            foreach(var edge in edges)
            {
                if (edge.nodeA == playerNode && edge.nodeB.id == node.id) edge.LineUI.GetComponent<Image>().sprite = linePastSprite;
            }
            node.ui.GetComponent<Animator>().SetBool("isSelectable", false);
            playerNode.ui.GetComponent<Animator>().SetBool("isPlayerUI", false);
            playerNode = node;
            NodeManager.Instance.ChangeNodeUI(playerUI, playerNode.id);
            if (playerNode.neighborIDs.Count > 0)
            {
                foreach (int nodeID in playerNode.neighborIDs)
                {
                    NodeManager.Instance.GetNodeByID(nodeID).ui.GetComponent<Button>().enabled = true;
                }
            }
            scrollView.transform.parent.gameObject.SetActive(false);
        });

        uiElement.GetComponent<Button>().enabled = false;

        nodeUIMap[node] = uiElement;
        node.ui = uiElement;
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

    NodeType DetermineNodeType(int x, int y)
    {
        float rand = Random.value;

        if (x >= 0 && x <= 3)
        {
            if (rand <= 0.5f) return NodeType.Battle_Normal;
            rand -= 0.5f;

            if (rand <= 0.35f) return NodeType.Quest;
            rand -= 0.35f;

            if (rand <= 0.15f) return NodeType.Market;
        }
        else if(x >= 5 && x <= 8)
        {
            if (rand <= 0.35f) return NodeType.Battle_Normal;
            rand -= 0.35f;

            if (rand <= 0.25f) return NodeType.Battle_Elite;
            rand -= 0.25f;

            if (rand <= 0.25f) return NodeType.Quest;
            rand -= 0.25f;

            if (rand <= 0.15f) return NodeType.Market;
        }

        return NodeType.Unknown;
    }

    void OnDrawGizmos()
    {
        if (nodes == null || edges == null) return;

        // Draw Paths
        Gizmos.color = Color.green;
        for (int i = 0; i < edges.Count; i++)
        {
            Gizmos.DrawLine(edges[i].nodeA.position, edges[i].nodeB.position);
        }

        // Draw Nodes
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x, y];
                if (node != null)
                {
                    Gizmos.color = GetNodeColor(node.type);
                    Gizmos.DrawSphere(node.position, 0.2f);
                }
            }
        }

        // Show start node and boss node
        Node startNode = new(-1, -1, new Vector2(-nodeSpacing, rows), NodeType.Battle_Normal, startMap);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startNode.position, 0.3f);

        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows), NodeType.Boss, bossMap);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(bossNode.position, 0.3f);
    }

    Color GetNodeColor(NodeType type)
    {
        switch (type)
        {
            case NodeType.Battle_Normal: return Color.red;
            case NodeType.Battle_Elite: return Color.magenta;
            case NodeType.Quest: return Color.blue;
            case NodeType.Treasure: return Color.yellow;
            case NodeType.Market: return Color.cyan;
            case NodeType.Comma: return Color.green;
            case NodeType.Boss: return Color.black;
            default: return Color.white;
        }
    }

    void HandleFirstColumnNodes(Node startNode, List<Node> firstColumnNodes)
    {
        // Randomly select nodes from column 1
        while (startNode.neighborIDs.Count < 5)
        {
            Node selectedNode = firstColumnNodes[Random.Range(0, firstColumnNodes.Count)];
            firstColumnNodes.Remove(selectedNode);

            // Connect with starting node
            float distance = Vector2.Distance(startNode.position, selectedNode.position);
            startNode.neighborIDs.Add(selectedNode.id);
            selectedNode.prevNode = startNode;
            edges.Add(new Edge(startNode, selectedNode, distance));
            nodeCount++;
            if (Random.value <= 0.25f && startNode.neighborIDs.Count > 1) break;
        }

        // Remove unselected nodes from column 1
        foreach (Node node in firstColumnNodes)
        {
            if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
            {
                Destroy(uiElement);
                nodeUIMap.Remove(node);
            }

            for (int y = 0; y < rows; y++)
            {
                if (nodes[0, y] == node) // Remove only nodes that match exactly
                {
                    nodes[0, y] = null;
                    NodeManager.Instance.RemoveNode(node);
                    break;
                }
            }
        }

        CreateNodeWorldMap(startNode);
    }

    void HandleFinalBossNode(Node bossNode)
    {
/*        // Connect the node in column 9 to the boss node
        for (int y = 0; y < rows; y++)
        {*/
        Node commaNode = nodes[columns - 1, rows / 2];
        float distance = Vector2.Distance(bossNode.position, commaNode.position);
        commaNode.neighborIDs.Add(bossNode.id);
        bossNode.prevNode = commaNode;
        edges.Add(new Edge(commaNode, bossNode, distance));
        CreateNodeWorldMap(commaNode);
        /*}*/

        CreateNodeWorldMap(bossNode);

        // Enable player location display and next map movement
        NodeManager.Instance.ChangeNodeUI(playerUI, startNode.id);
        foreach(int nodeID in startNode.neighborIDs)
        {
            NodeManager.Instance.GetNodeByID(nodeID).ui.GetComponent<Button>().enabled = true;
        }

        playerNode = startNode;

        DrawPaths();

        // ToDo: UI Changed;
        EventManager.Instance.PostNotification(EVENT_TYPE.Stage_Start, this, NodeManager.Instance.GetNodeByID(startNode.id));
        //EventManager.Instance.PostNotification(EVENT_TYPE.PageMapUIToGamePlay, this, NodeManager.Instance.GetNodeByID(startNode.id));

        if (!Utils.IsGraphConnected(startNode, nodes)) Debug.LogError("Graph disconnected");
    }

    void DrawPaths()
    {
        foreach(var edge in edges)
        {
            if(!nodeUIMap.ContainsKey(edge.nodeB)) continue;

            // World coordinates → screen coordinates conversion
            Vector3 screenPositionA = mainCamera.WorldToScreenPoint(new Vector3(edge.nodeA.position.x + 0.4f, edge.nodeA.position.y, 0f));
            Vector3 screenPositionB = mainCamera.WorldToScreenPoint(new Vector3(edge.nodeB.position.x - 0.4f, edge.nodeB.position.y, 0f));

            edge.LineUI = CreateLineUI(screenPositionA, screenPositionB);
        }

        mainCamera.transform.position = new Vector3(4, 10, -4);
        mainCamera.transform.Rotate(new Vector3(50, 0, 0));
        scrollView.transform.parent.gameObject.SetActive(false);
    }

    void ActivateNextPage(Portal portal)
    {
        playerNode.ui.GetComponent<Button>().enabled = false;
        foreach(var edge in edges)
        {
            if (edge.nodeA == playerNode) edge.LineUI.GetComponent<Image>().sprite = lineFutureSprite;
        }
        scrollView.transform.parent.gameObject.SetActive(true);
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