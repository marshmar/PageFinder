using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralMapGenerator : MonoBehaviour
{
    [Header("UI Generation Setting")]
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 10;
    [SerializeField] private float nodeSpacing = 2.0f;
    [SerializeField] private float offset = 0.1f;

    [Header("Appearance Probability")]
    [SerializeField] private float battleNormalProbability = 0.45f;
    [SerializeField] private float battleEliteProbability = 0.15f;
    [SerializeField] private float questProbability = 0.20f;
    [SerializeField] private float marketProbability = 0.10f;
    [SerializeField] private float commaProbability = 0.10f;

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

    public Node playerNode {get; private set;}

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(0, 2, -5);
        mainCamera.transform.rotation = Quaternion.Euler(Vector3.zero);

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
        HashSet<Node> activeNodes = new();

        // Add the node in the first column to the active node
        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null) activeNodes.Add(nodes[0, y]);
        }

        int[] rowNodeCount = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            rowNodeCount[i] = 1;
        }

        // Connect nodes by column
        for (int x = 0; x < columns - 1; x++)
        {
            HashSet<Node> nextActiveNodes = new();
            bool[] hasTwoNeighbors = { false, false };

            // Connecting each node to its neighbors
            foreach (Node currentNode in activeNodes)
            {
                List<Node> neighborCandidates = new();
                int currentY = currentNode.row; // Get the row of a node

                // Search for neighbor candidates in the next column (x+1)
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int nextY = currentY + offsetY;
                    if (nextY >= 0 && nextY < rows && nodes[x + 1, nextY] != null)
                    {
                        neighborCandidates.Add(nodes[x + 1, nextY]);
                    }
                }

                currentNode.type = DetermineNodeType(x, currentY);
                currentNode.map = nodeTypeWorldMap[currentNode.type];
                CreateNodeUI(currentNode);

                // Random neighbor connection
                while (neighborCandidates.Count > 0)
                {
                    // Neighbor candidate random selection
                    Node nextNode = neighborCandidates[Random.Range(0, neighborCandidates.Count)];
                    neighborCandidates.Remove(nextNode);

                    bool crossingDetected = false;
                    Vector2 currentPos = currentNode.position;
                    Vector2 nextPos = nextNode.position;

                    // Cross-check with existing path
                    foreach (var edge in edges)
                    {
                        if (Utils.IsCrossing(currentPos, nextPos, edge.nodeA.position, edge.nodeB.position))
                        {
                            crossingDetected = true;
                            break;
                        }
                    }

                    // If an intersection occurs, proceed to another candidate node.
                    if (crossingDetected) continue;

                    // Prevent more than 4 consecutive nodes in the same row
                    // if (currentY == nextNode.row && rowNodeCount[currentY] >= 4) ;

                    // Add a path (random error applied to distance)
                    float distance = Vector2.Distance(currentPos, nextPos) * Random.Range(1 - offset, 1 + offset);

                    // Neighborhood Candidate Connection
                    currentNode.neighborIDs.Add(nextNode.id);
                    nextNode.prevNode = currentNode;
                    edges.Add(new Edge(currentNode, nextNode, distance));
                    nextActiveNodes.Add(nextNode);

                    if (currentY == nextNode.row) rowNodeCount[currentY]++;

                    if (Random.value < 0.5f &&
                            !(x >= 3 && currentNode.prevNode.prevNode.prevNode.neighborIDs.Count == 1 &&
                            currentNode.prevNode.prevNode.neighborIDs.Count == 1 && currentNode.prevNode.neighborIDs.Count == 1)) // Single Connection
                    {
                        if (currentY != nextNode.row && currentNode.neighborIDs.Count == 1) rowNodeCount[currentY] = 1;
                        break;
                    }
                    else
                    {
                        if (currentNode.neighborIDs.Count == 2) break; // When there are two connected neighboring nodes
                        else if (System.Array.Exists(hasTwoNeighbors, n => !n)) // When dual connection is possible
                        {
                            if (!hasTwoNeighbors[0]) hasTwoNeighbors[0] = true;
                            else hasTwoNeighbors[1] = true;
                            continue;
                        }
                        else if (x >= 3 && currentNode.prevNode.prevNode.prevNode.neighborIDs.Count == 1 &&
                            currentNode.prevNode.prevNode.neighborIDs.Count == 1 &&
                            currentNode.prevNode.neighborIDs.Count == 1)
                        {
                            Debug.Log($"Exception: {currentNode.column},{currentNode.row}");
                        }
                        else break;
                    }
                }
                CreateNodeWorldMap(currentNode);
            }

            // Keep only active nodes in the next column
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x + 1, y];
                if (node != null && !nextActiveNodes.Contains(node))
                {
                    nodes[x + 1, y] = null;
                    NodeManager.Instance.RemoveNode(node);

                    if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
                    {
                        Destroy(uiElement);
                        nodeUIMap.Remove(node);
                    }
                }
            }

            activeNodes = nextActiveNodes;
        }

        HandleFinalBossNode(bossNode);
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
            EventManager.Instance.PostNotification(EVENT_TYPE.PageMapUIToGamePlay, this, node);

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
        if (x == 0) return NodeType.Battle_Normal;
        if (x == 4) return NodeType.Treasure;
        if (x == 9) return NodeType.Comma;
        if (x > 9) return NodeType.Boss;

        float rand = Random.value;

        if (rand < battleNormalProbability && x <= 1) return NodeType.Battle_Normal;
        rand -= battleNormalProbability;

        if (rand < battleEliteProbability && x >= 2) return NodeType.Battle_Elite;
        rand -= battleEliteProbability;

        if (rand < questProbability) return NodeType.Quest;
        rand -= questProbability;

        if (rand < marketProbability) return NodeType.Market;
        rand -= marketProbability;

        if(rand < commaProbability) return NodeType.Comma;
        rand -= commaProbability;

        return NodeType.Comma; // Default
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
        // Randomly select 2 nodes from column 1
        while (startNode.neighborIDs.Count < 2 && firstColumnNodes.Count > 0)
        {
            Node selectedNode = firstColumnNodes[Random.Range(0, firstColumnNodes.Count)];
            firstColumnNodes.Remove(selectedNode);

            // Connect with starting node
            float distance = Vector2.Distance(startNode.position, selectedNode.position);
            startNode.neighborIDs.Add(selectedNode.id);
            selectedNode.prevNode = startNode;
            edges.Add(new Edge(startNode, selectedNode, distance));
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
        // Connect the node in column 10 to the boss node
        for (int y = 0; y < rows; y++)
        {
            Node node = nodes[columns - 1, y];
            if (node is not null)
            {
                node.type = NodeType.Comma;
                CreateNodeUI(node);
                float distance = Vector2.Distance(bossNode.position, node.position);
                node.neighborIDs.Add(bossNode.id);
                bossNode.prevNode = node;
                edges.Add(new Edge(node, bossNode, distance));
                CreateNodeWorldMap(node);
            }
        }

        CreateNodeWorldMap(bossNode);

        // Enable player location display and next map movement
        NodeManager.Instance.ChangeNodeUI(playerUI, startNode.id);
        foreach(int nodeID in startNode.neighborIDs)
        {
            NodeManager.Instance.GetNodeByID(nodeID).ui.GetComponent<Button>().enabled = true;
        }

        playerNode = startNode;

        DrawPaths();

        EventManager.Instance.PostNotification(EVENT_TYPE.PageMapUIToGamePlay, this, NodeManager.Instance.GetNodeByID(startNode.id));

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
}