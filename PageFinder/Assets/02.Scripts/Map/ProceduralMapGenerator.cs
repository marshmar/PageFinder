using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralMapGenerator : MonoBehaviour
{
    [Header("UI Generation Setting")]
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 10;
    [SerializeField] private float nodeSpacing = 7.0f;
    [SerializeField] private float offset = 0.1f;

    [Header("Appearance Probability")]
    [SerializeField] private float battleNormalProbability = 0.45f;
    [SerializeField] private float battleEliteProbability = 0.15f;
    [SerializeField] private float questProbability = 0.20f;
    [SerializeField] private float marketProbability = 0.10f;
    [SerializeField] private float commaProbability = 0.10f;

    [Header("UI Setting")]
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private GameObject battleNormalPrefab;
    [SerializeField] private GameObject battleElitePrefab;
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject marketPrefab;
    [SerializeField] private GameObject commaPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject lineUIPrefab;

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
    private Dictionary<Node, GameObject> nodeUIMap = new(); // 노드와 UI 매핑
    private Dictionary<Node, GameObject> worldMapInstances = new(); // 노드와 맵 인스턴스를 매핑
    private Dictionary<NodeType, GameObject> nodeTypeUIMap;
    private Dictionary<NodeType, GameObject> nodeTypeWorldMap;

    [SerializeField] private GameObject player;

    void Start()
    {
        mainCamera = Camera.main;
        nodeTypeUIMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Start, battleNormalPrefab },
            { NodeType.Battle_Normal, battleNormalPrefab },
            { NodeType.Battle_Elite, battleElitePrefab },
            { NodeType.Quest, questPrefab },
            { NodeType.Treasure, treasurePrefab },
            { NodeType.Market, marketPrefab },
            { NodeType.Comma, commaPrefab },
            { NodeType.Boss, bossPrefab }
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

        // 1열 이전의 시작 노드 생성
        startNode = new(-1, rows / 2, new Vector2(-nodeSpacing, rows), NodeType.Start, nodeTypeWorldMap[NodeType.Start]);
        NodeManager.Instance.AddNode(startNode);
        CreateNodeUI(startNode);

        // 1~10열 노드 생성
        List<Node> firstColumnNodes = new();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new(x * nodeSpacing, y * 1.6f * Random.Range(1 - offset, 1 + offset));

                Node newNode = new(x, y, position, NodeType.Unknown, nodeTypeWorldMap[NodeType.Start]);
                NodeManager.Instance.AddNode(newNode);
                nodes[x, y] = newNode;

                if (x == 0) firstColumnNodes.Add(newNode);
            }
        }

        // 10열 이후의 최종 보스전 노드 생성
        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows), NodeType.Boss, nodeTypeWorldMap[NodeType.Boss]);
        NodeManager.Instance.AddNode(bossNode);
        CreateNodeUI(bossNode);

        HandleFirstColumnNodes(startNode, firstColumnNodes);

        ConnectNodes(bossNode);
    }

    void ConnectNodes(Node bossNode)
    {
        HashSet<Node> activeNodes = new();

        // 첫 번째 열의 활성 노드 추가
        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null) activeNodes.Add(nodes[0, y]);
        }

        int[] rowNodeCount = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            rowNodeCount[i] = 1;
        }

        // 열(column) 기준으로 노드 연결
        for (int x = 0; x < columns - 1; x++)
        {
            HashSet<Node> nextActiveNodes = new();
            bool[] hasTwoNeighbors = { false, false };

            // 각 노드의 이웃 노드 연결
            foreach (Node currentNode in activeNodes)
            {
                List<Node> neighborCandidates = new();
                int currentY = currentNode.row; // 노드의 행(row) 가져오기

                // 다음 열(x+1)에서 이웃 후보 탐색
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

                // 무작위 이웃 연결
                while (neighborCandidates.Count > 0)
                {
                    // 이웃 후보 랜덤 선택
                    Node nextNode = neighborCandidates[Random.Range(0, neighborCandidates.Count)];
                    neighborCandidates.Remove(nextNode);

                    bool crossingDetected = false;
                    Vector2 currentPos = currentNode.position;
                    Vector2 nextPos = nextNode.position;

                    // 기존 경로와 교차 검사
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

                    // 교차가 발생하면 다른 후보 노드로 진행
                    if (crossingDetected) continue;

                    // 같은 행에서 4연속 이상인 노드 방지
                    // if (currentY == nextNode.row && rowNodeCount[currentY] >= 4) ;

                    // 경로 추가 (거리에 랜덤 오차 적용)
                    float distance = Vector2.Distance(currentPos, nextPos) * Random.Range(1 - offset, 1 + offset);

                    // 이웃 후보 연결
                    currentNode.neighborIDs.Add(nextNode.id);
                    nextNode.prevNode = currentNode;
                    edges.Add((currentNode, nextNode, distance));
                    nextActiveNodes.Add(nextNode);

                    if (currentY == nextNode.row) rowNodeCount[currentY]++;

                    if (Random.value < 0.5f &&
                            !(x >= 3 && currentNode.prevNode.prevNode.prevNode.neighborIDs.Count == 1 &&
                            currentNode.prevNode.prevNode.neighborIDs.Count == 1 && currentNode.prevNode.neighborIDs.Count == 1)) // 단일 연결
                    {
                        if (currentY != nextNode.row && currentNode.neighborIDs.Count == 1) rowNodeCount[currentY] = 1;
                        break;
                    }
                    else
                    {
                        if (currentNode.neighborIDs.Count == 2) break; // 연결된 이웃 노드가 2개일 경우
                        else if (System.Array.Exists(hasTwoNeighbors, n => !n)) // 이중 연결이 가능한 경우
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

            // 다음 열의 활성 노드만 유지
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
        // UI 노드의 스크린 좌표를 월드 좌표로 변환
        if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
        {
            Vector3 screenPosition = uiElement.GetComponent<RectTransform>().position;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

            // 노드 위치에 맵 프리팹 생성 및 간격 적용
            Vector3 adjustedPosition = new Vector3(worldPosition.x, 0f, 0f) + new Vector3(node.row* 100, 0, node.column * 100);
            GameObject mapInstance = Instantiate(node.map, adjustedPosition, Quaternion.identity, this.transform);
            worldMapInstances[node] = mapInstance;
            node.map.GetComponent<Map>().position = adjustedPosition;

            if(node == startNode) player.transform.position = adjustedPosition + Vector3.up * 1.1f;

            // 각 노드의 이웃 노드로 가는 포탈 생성
            foreach (int neighborID in node.neighborIDs)
            {
                Node neighbor = NodeManager.Instance.GetNodeByID(neighborID);
                CreatePortal(node, neighbor);
            }
        }
    }

    void CreatePortal(Node currentNode, Node targetNode)
    {
        // 현재 노드 맵 프리팹이 있는지 확인
        if (worldMapInstances.TryGetValue(currentNode, out GameObject currentMap))
        {
            // 맵 내부 포탈 위치 설정
            Vector3 portalPosition = currentMap.transform.position + new Vector3(0, 2, 8);

            // 포탈 생성 및 맵 프리팹 내부에 배치
            GameObject portal = Instantiate(portalPrefab, portalPosition, Quaternion.identity, currentMap.transform);
            currentNode.portal = portal.GetComponent<Portal>();

            if(currentNode == startNode) Portal.OnPortalEnter += ActivateNextPage;
        }
    }

    void CreateNodeUI(Node node)
    {
        if (!nodeTypeUIMap.TryGetValue(node.type, out GameObject selectedPrefab) || selectedPrefab == null)
        {
            Debug.LogWarning($"No prefab found for NodeType: {node.type}");
            return;
        }

        // 노드의 위치를 Screen Space로 변환하여 UI 배치
        Vector3 worldPosition = new(node.position.x, node.position.y, 0f);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        GameObject uiElement = Instantiate(selectedPrefab, scrollView.content);

        uiElement.GetComponent<RectTransform>().position = screenPosition;

        uiElement.GetComponent<Button>().onClick.AddListener(() => {
            Portal.Teleport(node.map.GetComponent<Map>().position);
            scrollView.transform.parent.gameObject.SetActive(false);
        });

        nodeUIMap[node] = uiElement;
    }

    void CreateLineUI(Vector3 start, Vector3 end)
    {
        GameObject lineObject = Instantiate(lineUIPrefab, scrollView.content);
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();

        // 선의 중심 위치 설정
        rectTransform.position = (start + end) / 2;

        // 선의 두께 및 길이 설정
        float distance = Vector3.Distance(start, end);
        rectTransform.sizeDelta = new Vector2(distance, 20);

        // 선의 회전 설정
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    NodeType DetermineNodeType(int x, int y)
    {
        if (x == 0) return NodeType.Battle_Normal; // 1열: 일반 배틀
        if (x == 4) return NodeType.Treasure; // 5열: 트레저
        if (x == 9) return NodeType.Comma; // 10열: 콤마
        if (x > 9) return NodeType.Boss; // 10열 이후: 보스

        float rand = Random.value;

        if (rand < battleNormalProbability && x <= 1) return NodeType.Battle_Normal; // 일반 배틀 등장 조건
        rand -= battleNormalProbability;

        if (rand < battleEliteProbability && x >= 2) return NodeType.Battle_Elite; // 정예 배틀 등장 조건
        rand -= battleEliteProbability;

        if (rand < questProbability) return NodeType.Quest;
        rand -= questProbability;

        if (rand < marketProbability) return NodeType.Market;
        rand -= marketProbability;

        if(rand < commaProbability) return NodeType.Comma;
        rand -= commaProbability;

        return NodeType.Comma; // 기본값
    }

    void OnDrawGizmos()
    {
        if (nodes == null || edges == null) return;

        // 경로 그리기
        Gizmos.color = Color.green;
        for (int i = 0; i < edges.Count; i++)
        {
            Gizmos.DrawLine(edges[i].Item1.position, edges[i].Item2.position);
        }

        // 노드 그리기
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

        // 시작 노드와 보스 노드 표시
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
        // 1열에서 무작위로 2개의 노드를 선택
        while (startNode.neighborIDs.Count < 2 && firstColumnNodes.Count > 0)
        {
            Node selectedNode = firstColumnNodes[Random.Range(0, firstColumnNodes.Count)];
            firstColumnNodes.Remove(selectedNode);

            // 시작 노드와 연결
            float distance = Vector2.Distance(startNode.position, selectedNode.position);
            startNode.neighborIDs.Add(selectedNode.id);
            edges.Add((startNode, selectedNode, distance));
        }

        // 1열에서 선택되지 않은 노드 제거
        foreach (Node node in firstColumnNodes)
        {
            if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
            {
                Destroy(uiElement);
                nodeUIMap.Remove(node);
            }

            for (int y = 0; y < rows; y++)
            {
                if (nodes[0, y] == node) // 정확히 매칭되는 노드만 제거
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
        // 10열에 존재하는 노드를 보스전 노드와 연결
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

        DrawPaths();

        if (!Utils.IsGraphConnected(startNode, nodes)) Debug.LogError("그래프 연결 끊어짐");
    }

    void DrawPaths()
    {
        foreach (var edge in edges)
        {
            Node nodeA = edge.Item1;
            Node nodeB = edge.Item2;
            if(!nodeUIMap.ContainsKey(nodeB)) continue;

            // 월드 좌표 → 스크린 좌표 변환
            Vector3 screenPositionA = mainCamera.WorldToScreenPoint(new Vector3(nodeA.position.x + 0.4f, nodeA.position.y, 0f));
            Vector3 screenPositionB = mainCamera.WorldToScreenPoint(new Vector3(nodeB.position.x - 0.4f, nodeB.position.y, 0f));

            CreateLineUI(screenPositionA, screenPositionB);
        }

        scrollView.transform.parent.gameObject.SetActive(false);
    }

    void ActivateNextPage(Portal portal)
    {
        scrollView.transform.parent.gameObject.SetActive(true);
        // 추가 로직 구현 예정
    }
}