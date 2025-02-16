using System.Collections.Generic;
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
    private List<(Node, Node, float)> edges = new();
    private Dictionary<Node, GameObject> nodeUIMap = new(); // ���� UI ����
    private Dictionary<Node, GameObject> worldMapInstances = new(); // ���� �� �ν��Ͻ��� ����
    private Dictionary<NodeType, GameObject> nodeTypeUIMap;
    private Dictionary<NodeType, GameObject> nodeTypeWorldMap;

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

        // 1�� ������ ���� ��� ����
        startNode = new(-1, rows / 2, new Vector2(-nodeSpacing, rows/2), NodeType.Start, nodeTypeWorldMap[NodeType.Start]);
        NodeManager.Instance.AddNode(startNode);
        CreateNodeUI(startNode);

        // 1~10�� ��� ����
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

        // 10�� ������ ���� ������ ��� ����
        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows/2), NodeType.Boss, nodeTypeWorldMap[NodeType.Boss]);
        NodeManager.Instance.AddNode(bossNode);
        CreateNodeUI(bossNode);

        HandleFirstColumnNodes(startNode, firstColumnNodes);

        ConnectNodes(bossNode);
    }

    void ConnectNodes(Node bossNode)
    {
        HashSet<Node> activeNodes = new();

        // ù ��° ���� Ȱ�� ��� �߰�
        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null) activeNodes.Add(nodes[0, y]);
        }

        int[] rowNodeCount = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            rowNodeCount[i] = 1;
        }

        // ��(column) �������� ��� ����
        for (int x = 0; x < columns - 1; x++)
        {
            HashSet<Node> nextActiveNodes = new();
            bool[] hasTwoNeighbors = { false, false };

            // �� ����� �̿� ��� ����
            foreach (Node currentNode in activeNodes)
            {
                List<Node> neighborCandidates = new();
                int currentY = currentNode.row; // ����� ��(row) ��������

                // ���� ��(x+1)���� �̿� �ĺ� Ž��
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

                // ������ �̿� ����
                while (neighborCandidates.Count > 0)
                {
                    // �̿� �ĺ� ���� ����
                    Node nextNode = neighborCandidates[Random.Range(0, neighborCandidates.Count)];
                    neighborCandidates.Remove(nextNode);

                    bool crossingDetected = false;
                    Vector2 currentPos = currentNode.position;
                    Vector2 nextPos = nextNode.position;

                    // ���� ��ο� ���� �˻�
                    foreach (var (startNode, endNode, _) in edges)
                    {
                        Vector2 existingStart = startNode.position;
                        Vector2 existingEnd = endNode.position;

                        if (Utils.IsCrossing(currentPos, nextPos, existingStart, existingEnd))
                        {
                            crossingDetected = true;
                            break;
                        }
                    }

                    // ������ �߻��ϸ� �ٸ� �ĺ� ���� ����
                    if (crossingDetected) continue;

                    // ���� �࿡�� 4���� �̻��� ��� ����
                    // if (currentY == nextNode.row && rowNodeCount[currentY] >= 4) ;

                    // ��� �߰� (�Ÿ��� ���� ���� ����)
                    float distance = Vector2.Distance(currentPos, nextPos) * Random.Range(1 - offset, 1 + offset);

                    // �̿� �ĺ� ����
                    currentNode.neighborIDs.Add(nextNode.id);
                    nextNode.prevNode = currentNode;
                    edges.Add((currentNode, nextNode, distance));
                    nextActiveNodes.Add(nextNode);

                    if (currentY == nextNode.row) rowNodeCount[currentY]++;

                    if (Random.value < 0.5f &&
                            !(x >= 3 && currentNode.prevNode.prevNode.prevNode.neighborIDs.Count == 1 &&
                            currentNode.prevNode.prevNode.neighborIDs.Count == 1 && currentNode.prevNode.neighborIDs.Count == 1)) // ���� ����
                    {
                        if (currentY != nextNode.row && currentNode.neighborIDs.Count == 1) rowNodeCount[currentY] = 1;
                        break;
                    }
                    else
                    {
                        if (currentNode.neighborIDs.Count == 2) break; // ����� �̿� ��尡 2���� ���
                        else if (System.Array.Exists(hasTwoNeighbors, n => !n)) // ���� ������ ������ ���
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

            // ���� ���� Ȱ�� ��常 ����
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
        // UI ����� ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
        if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
        {
            Vector3 screenPosition = uiElement.GetComponent<RectTransform>().position;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

            /*
             *  // ���� �ڵ�
             *  // ��� ��ġ�� �� ������ ���� �� ���� ����
                Vector3 adjustedPosition = new Vector3(worldPosition.x, 0f, 0f) + new Vector3(node.row* 100, 0, node.column * 100);
                GameObject mapInstance = Instantiate(node.map, adjustedPosition, Quaternion.identity, this.transform);
                worldMapInstances[node] = mapInstance;
                node.map.GetComponent<Map>().position = adjustedPosition;

             *  if(node == startNode) player.transform.position = adjustedPosition + Vector3.up * 1.1f;
             */


            // �ֽ�ǥ ����
            //---------------------------------------------------------------------------------------------------------------------------------
            // ��� ��ġ�� �� ������ ���� �� ���� ����
            Vector3 adjustedPosition = new Vector3(worldPosition.x, 0f, 0f) + new Vector3(node.row* 100, 0, node.column * 100);
            GameObject mapInstance = Instantiate(node.map, adjustedPosition, Quaternion.Euler(0, 90, 0), this.transform);
            worldMapInstances[node] = mapInstance;
            node.map.GetComponent<Map>().position = adjustedPosition;
            node.map = mapInstance;

            // ���� ���� �´� �� ���� ����
            PhaseData phaseData = mapInstance.GetComponentInChildren<PhaseData>();

            Debug.Log($"------------{node.id}---------------");
            // ��Ʋ ������ ���� ���� PhaseDatas�� ����
            // PhaseDatas�� �����ϴ� �͸� �� ����
            if (phaseData)
                phaseData.SetEnemyDatas(adjustedPosition, node.column);

            if (node == startNode)
            {
                GameObject player = GameObject.FindGameObjectWithTag("PLAYER");
                if(player is not null)
                    player.transform.position = mapInstance.transform.GetChild(0).position;  // Map�� ù ��° �ڽ� ��ü : PlayerPos
            }
            //---------------------------------------------------------------------------------------------------------------------------------

            // �� ����� �̿� ���� ���� ��Ż ����
            foreach (int neighborID in node.neighborIDs)
            {
                Node neighbor = NodeManager.Instance.GetNodeByID(neighborID);
                CreatePortal(node, neighbor);
            }
        }
    }

    void CreatePortal(Node currentNode, Node targetNode)
    {
        // ���� ��� �� �������� �ִ��� Ȯ��
        if (worldMapInstances.TryGetValue(currentNode, out GameObject currentMap))
        {
            // �� ���� ��Ż ��ġ ����
            //Vector3 portalPosition = currentMap.transform.GetChild(1).position; // �� �������� 2��° �ڽİ�ü : ��Ż ��ġ
            Vector3 portalPosition = currentMap.transform.position + new Vector3(8, 2, 0);

            // ��Ż ���� �� �� ������ ���ο� ��ġ
            GameObject portal = Instantiate(portalPrefab, portalPosition, Quaternion.identity, currentMap.transform);
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

        // ����� ��ġ�� Screen Space�� ��ȯ�Ͽ� UI ��ġ
        Vector3 worldPosition = new(node.position.x, node.position.y, 0f);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        GameObject uiElement = Instantiate(selectedPrefab, scrollView.content);

        uiElement.GetComponent<RectTransform>().position = screenPosition;

        uiElement.GetComponent<Button>().onClick.AddListener(() => {
            //Portal.Teleport(node.map.GetComponent<Map>().position + Vector3.up * 1.1f);
            Portal.Teleport(node.map.transform.GetChild(0).position); // Map�� ù ��° �ڽ� ��ü : PlayerPos
            EventManager.Instance.PostNotification(EVENT_TYPE.PageMapUIToGamePlay, this, node); // ��� ���� �޾Ƽ� GameData���� ���� ����
            scrollView.transform.parent.gameObject.SetActive(false);
            //NodeManager.Instance.ChangeNodeUI(battleNormalUI, node.prevNode.id);
            NodeManager.Instance.ChangeNodeUI(playerUI, node.id);
        });

        nodeUIMap[node] = uiElement;
        node.ui = uiElement;
    }

    void CreateLineUI(Vector3 start, Vector3 end)
    {
        GameObject lineObject = Instantiate(lineUI, scrollView.content);
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();

        // ���� �߽� ��ġ ����
        rectTransform.position = (start + end) / 2;

        // ���� �β� �� ���� ����
        float distance = Vector3.Distance(start, end);
        rectTransform.sizeDelta = new Vector2(distance, 20);

        // ���� ȸ�� ����
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    NodeType DetermineNodeType(int x, int y)
    {
        if (x == 0) return NodeType.Battle_Normal; // 1��: �Ϲ� ��Ʋ
        if (x == 4) return NodeType.Treasure; // 5��: Ʈ����
        if (x == 9) return NodeType.Comma; // 10��: �޸�
        if (x > 9) return NodeType.Boss; // 10�� ����: ����

        float rand = Random.value;

        if (rand < battleNormalProbability && x <= 1) return NodeType.Battle_Normal; // �Ϲ� ��Ʋ ���� ����
        rand -= battleNormalProbability;

        if (rand < battleEliteProbability && x >= 2) return NodeType.Battle_Elite; // ���� ��Ʋ ���� ����
        rand -= battleEliteProbability;

        if (rand < questProbability) return NodeType.Quest;
        rand -= questProbability;

        if (rand < marketProbability) return NodeType.Market;
        rand -= marketProbability;

        if(rand < commaProbability) return NodeType.Comma;
        rand -= commaProbability;

        return NodeType.Comma; // �⺻��
    }

    void OnDrawGizmos()
    {
        if (nodes == null || edges == null) return;

        // ��� �׸���
        Gizmos.color = Color.green;
        for (int i = 0; i < edges.Count; i++)
        {
            Gizmos.DrawLine(edges[i].Item1.position, edges[i].Item2.position);
        }

        // ��� �׸���
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

        // ���� ���� ���� ��� ǥ��
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
        // 1������ �������� 2���� ��带 ����
        while (startNode.neighborIDs.Count < 2 && firstColumnNodes.Count > 0)
        {
            Node selectedNode = firstColumnNodes[Random.Range(0, firstColumnNodes.Count)];
            firstColumnNodes.Remove(selectedNode);

            // ���� ���� ����
            float distance = Vector2.Distance(startNode.position, selectedNode.position);
            startNode.neighborIDs.Add(selectedNode.id);
            edges.Add((startNode, selectedNode, distance));
        }

        // 1������ ���õ��� ���� ��� ����
        foreach (Node node in firstColumnNodes)
        {
            if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
            {
                Destroy(uiElement);
                nodeUIMap.Remove(node);
            }

            for (int y = 0; y < rows; y++)
            {
                if (nodes[0, y] == node) // ��Ȯ�� ��Ī�Ǵ� ��常 ����
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
        // 10���� �����ϴ� ��带 ������ ���� ����
        for (int y = 0; y < rows; y++)
        {
            Node node = nodes[columns - 1, y];
            if (node is not null)
            {
                node.type = NodeType.Comma;
                CreateNodeUI(node);
                float distance = Vector2.Distance(bossNode.position, node.position);
                node.neighborIDs.Add(bossNode.id);
                edges.Add((node, bossNode, distance));
                CreateNodeWorldMap(node);
            }
        }

        CreateNodeWorldMap(bossNode);

        NodeManager.Instance.ChangeNodeUI(playerUI, startNode.id);

        DrawPaths();


        // �ֽ�ǥ �߰�
        // Start Node�� ������ �ѱ����� ���� ����
        EventManager.Instance.PostNotification(EVENT_TYPE.PageMapUIToGamePlay, this, NodeManager.Instance.GetNodeByID(0)); // ��� ���� �޾Ƽ� GameData���� ���� ����

        if (!Utils.IsGraphConnected(startNode, nodes)) Debug.LogError("�׷��� ���� ������");
    }

    void DrawPaths()
    {
        foreach (var edge in edges)
        {
            Node nodeA = edge.Item1;
            Node nodeB = edge.Item2;
            if(!nodeUIMap.ContainsKey(nodeB)) continue;

            // ���� ��ǥ �� ��ũ�� ��ǥ ��ȯ
            Vector3 screenPositionA = mainCamera.WorldToScreenPoint(new Vector3(nodeA.position.x + 0.4f, nodeA.position.y, 0f));
            Vector3 screenPositionB = mainCamera.WorldToScreenPoint(new Vector3(nodeB.position.x - 0.4f, nodeB.position.y, 0f));

            CreateLineUI(screenPositionA, screenPositionB);
        }

        mainCamera.transform.position = new Vector3(4, 10, -4);
        mainCamera.transform.Rotate(new Vector3(50, 0, 0));
        scrollView.transform.parent.gameObject.SetActive(false);
    }

    void ActivateNextPage(Portal portal)
    {
        scrollView.transform.parent.gameObject.SetActive(true);
        // �߰� ���� ���� ����
    }

    // �ֽ�ǥ �߰�
    public GameObject GetMapInstance(Node node)
    {
        return worldMapInstances[node];
    }
}