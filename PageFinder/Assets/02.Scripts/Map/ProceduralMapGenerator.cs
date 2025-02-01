using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 10;
    [Tooltip("노드 간격 조정")]
    [SerializeField] private float nodeSpacing = 2.0f;
    [SerializeField] private float minOffset = 0.9f;
    [SerializeField] private float maxOffset = 1.1f;

    public enum NodeType { Unknown, Battle_Normal, Battle_Elite, Quest, Treasure, Market, Comma, Boss }

    public class Node
    {
        public int column;
        public int row;
        public Vector2 position;
        public NodeType type;
        public List<Node> Neighbors = new();
        public Node prevNode;
        public GameObject map;

        public Node(int column, int row, Vector2 position, NodeType type, GameObject map)
        {
            this.column = column;
            this.row = row;
            this.position = position;
            this.type = type;
            this.map = map;
        }
    }

    public Node[,] nodes; // 맵 프리팹과 매핑하여 이웃 노드를 포탈 이동에 활용할 예정
    private Node startNode;
    private List<(Node, Node, float)> edges = new();

    [Header("Appearance Probability")]
    [SerializeField] private float battleNormalProbability = 0.45f;
    [SerializeField] private float battleEliteProbability = 0.15f;
    [SerializeField] private float questProbability = 0.20f;
    [SerializeField] private float treasureProbability = 0.0f;
    [SerializeField] private float marketProbability = 0.10f;
    [SerializeField] private float commaProbability = 0.10f;

    [Header("UI Setting")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject battleNormalPrefab;
    [SerializeField] private GameObject battleElitePrefab;
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject marketPrefab;
    [SerializeField] private GameObject commaPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject lineUIPrefab;
    
    private Dictionary<Node, GameObject> nodeUIMap = new(); // 노드와 UI 매핑
    private List<GameObject> activeLines = new();
    private Dictionary<NodeType, GameObject> prefabMap;

    [Header("Map Setting")]
    [SerializeField] private GameObject testMap;

    void Start()
    {
        prefabMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Battle_Normal, battleNormalPrefab },
            { NodeType.Battle_Elite, battleElitePrefab },
            { NodeType.Quest, questPrefab },
            { NodeType.Treasure, treasurePrefab },
            { NodeType.Market, marketPrefab },
            { NodeType.Comma, commaPrefab },
            { NodeType.Boss, bossPrefab }
        };

        GenerateNodes();
    }

    void GenerateNodes()
    {
        nodes = new Node[columns, rows];

        // 1열 이전의 시작 노드 생성
        startNode = new(-1, rows / 2, new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal, testMap);
        CreateNodeUI(startNode);

        // 1~10열 노드 생성
        List<Node> firstColumnNodes = new();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new(x * nodeSpacing, y * nodeSpacing * Random.Range(minOffset, maxOffset));

                Node newNode = new(x, y, position, NodeType.Unknown, testMap);
                nodes[x, y] = newNode;

                if (x == 0) firstColumnNodes.Add(newNode);
            }
        }

        // 10열 이후의 최종 보스전 노드 생성
        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss, testMap);
        CreateNodeUI(bossNode);

        HandleFirstColumnNodes(startNode, firstColumnNodes);

        ConnectNodes(bossNode);
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

        return NodeType.Comma; // 기본값
    }

    void CreateNodeUI(Node node)
    {
        if (!prefabMap.TryGetValue(node.type, out GameObject selectedPrefab) || selectedPrefab == null)
        {
            Debug.LogWarning($"No prefab found for NodeType: {node.type}");
            return;
        }

        // 노드의 위치를 Screen Space로 변환하여 UI 배치
        Vector3 worldPosition = new(node.position.x, node.position.y, 0f);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        GameObject uiElement = Instantiate(selectedPrefab, canvas.transform);

        uiElement.GetComponent<RectTransform>().position = screenPosition;
        nodeUIMap[node] = uiElement;
    }

    void ConnectNodes(Node bossNode)
    {
        HashSet<Node> activeNodes = new();

        // 첫 번째 열의 활성 노드 추가
        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null) activeNodes.Add(nodes[0, y]);
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
                CreateNodeUI(currentNode);

                // 무작위 이웃 연결
                while (neighborCandidates.Count > 0)
                {
                    // 이웃 후보 랜덤 선택
                    Node nextNode = neighborCandidates[Random.Range(0, neighborCandidates.Count)];
                    neighborCandidates.Remove(nextNode);

                    // 경로 추가 (거리에 랜덤 오차 적용)
                    float distance = Vector2.Distance(currentNode.position, nextNode.position);
                    distance *= Random.Range(minOffset, maxOffset);

                    // 이웃 후보 연결
                    currentNode.Neighbors.Add(nextNode);
                    nextNode.prevNode = currentNode;
                    edges.Add((currentNode, nextNode, distance));
                    nextActiveNodes.Add(nextNode);

                    if (Random.value < 0.5f &&
                            !(x >= 2 && currentNode.prevNode.prevNode.Neighbors.Count == 1 && currentNode.prevNode.Neighbors.Count == 1)) // 단일 연결
                    {
                        Debug.Log($"Single: {currentNode.column},{currentNode.row}");
                        break;
                    }
                    else
                    {
                        if (currentNode.Neighbors.Count == 2) break; // 연결된 이웃 노드가 2개일 경우
                        else if (System.Array.Exists(hasTwoNeighbors, n => !n)) // 이중 연결이 가능한 경우
                        {
                            if (!hasTwoNeighbors[0]) hasTwoNeighbors[0] = true;
                            else hasTwoNeighbors[1] = true;
                            Debug.Log($"Double: {currentNode.column},{currentNode.row}");
                            continue;
                        }
                        else if (x >= 2 && currentNode.prevNode.prevNode.Neighbors.Count == 1 && currentNode.prevNode.Neighbors.Count == 1) Debug.Log($"Exception: {currentNode.column},{currentNode.row}");
                        else break;
                    }
                }
            }

            // 다음 열의 활성 노드만 유지
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x + 1, y];
                if (node != null && !nextActiveNodes.Contains(node))
                {
                    nodes[x + 1, y] = null;

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
        Node startNode = new(-1, -1, new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal, testMap);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startNode.position, 0.3f);

        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss, testMap);
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
        // 1열에서 무작위로 최대 2개의 노드를 선택
        while (startNode.Neighbors.Count < 2 && firstColumnNodes.Count > 0)
        {
            Node selectedNode = firstColumnNodes[Random.Range(0, firstColumnNodes.Count)];
            firstColumnNodes.Remove(selectedNode);

            // 연결 추가
            float distance = Vector2.Distance(startNode.position, selectedNode.position);
            startNode.Neighbors.Add(selectedNode);
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
                    Debug.Log($"Removed unselected node at position: {node.position}");
                    break;
                }
            }
        }
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
                node.Neighbors.Add(bossNode);
                edges.Add((node, bossNode, distance));
            }
        }

        DrawPaths();

        if (!IsGraphConnected())
        {
            Debug.LogError("그래프가 연결되어 있지 않습니다!");
        }
    }

    bool IsGraphConnected()
    {
        HashSet<Node> visited = new();
        Stack<Node> stack = new();

        if (startNode == null)
        {
            Debug.LogError("No active nodes found. Cannot check connectivity.");
            return false;
        }

        // DFS 탐색
        stack.Push(startNode);
        visited.Add(startNode);
        
        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();

            foreach (Node neighbor in currentNode.Neighbors)
            {
                if (neighbor != null && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }
            }
        }

        // 활성화된 모든 노드 방문 여부 재확인
        foreach (Node node in nodes.Cast<Node>().Where(n => n != null))
        {
            if (!visited.Contains(node))
            {
                Debug.LogError($"Node at position {node.position} is not connected.");
                return false;
            }
        }

        return true;
    }

    void DrawPaths()
    {
        foreach (var edge in edges)
        {
            Node nodeA = edge.Item1;
            Node nodeB = edge.Item2;

            // 월드 좌표 → 스크린 좌표 변환
            Vector3 screenPositionA = mainCamera.WorldToScreenPoint(new Vector3(nodeA.position.x + 0.5f, nodeA.position.y, 0f));
            Vector3 screenPositionB = mainCamera.WorldToScreenPoint(new Vector3(nodeB.position.x - 0.5f, nodeB.position.y, 0f));

            CreateLineUI(screenPositionA, screenPositionB);
        }
    }

    void CreateLineUI(Vector3 start, Vector3 end)
    {
        GameObject lineObject = Instantiate(lineUIPrefab, canvas.transform);
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();

        // 선의 중심 위치 설정
        rectTransform.position = (start + end) / 2;

        // 선의 길이 설정
        float distance = Vector3.Distance(start, end);
        rectTransform.sizeDelta = new Vector2(distance, 5f);  // 두께 5, 길이는 두 노드 사이 거리로 설정

        // 선의 회전 설정
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        activeLines.Add(lineObject);  // 나중에 제거할 수 있도록 관리
    }
}