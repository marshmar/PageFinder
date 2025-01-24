using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 10;
    [SerializeField] private float nodeSpacing = 2.0f; // 거리 조정

    public enum NodeType { Battle_Normal, Battle_Elite, Quest, Treasure, Market, Comma, Boss }

    public class Node
    {
        public Vector2 Position;
        public NodeType Type;
        public List<Node> Neighbors = new List<Node>();

        public Node(Vector2 position, NodeType type)
        {
            Position = position;
            Type = type;
        }
    }

    private Node[,] nodes;
    private List<(Node, Node, float)> edges = new List<(Node, Node, float)>(); // 노드 간 거리 포함

    [Header("Appearance Probability")]
    [SerializeField] private float battleNormalProbability = 0.45f;
    [SerializeField] private float battleEliteProbability = 0.15f;
    [SerializeField] private float questProbability = 0.20f;
    [SerializeField] private float treasureProbability = 0.0f;
    [SerializeField] private float marketProbability = 0.10f;
    [SerializeField] private float commaProbability = 0.10f;

    void Start()
    {
        GenerateNodes();
    }

    void GenerateNodes()
    {
        nodes = new Node[columns, rows];

        // 1열 이전의 시작 노드 생성
        Node startNode = new Node(new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal);
        List<Node> firstColumnNodes = new List<Node>();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new Vector2(x * nodeSpacing, y * nodeSpacing);
                NodeType type = DetermineNodeType(x, y);

                Node newNode = new Node(position, type);
                nodes[x, y] = newNode;

                // 1열 노드 추가
                if (x == 0) firstColumnNodes.Add(newNode);
            }
        }

        // 10열 이후의 최종 보스전 노드 생성
        Node bossNode = new Node(new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss);

        // 1열과 10열 처리
        HandleFirstColumnNodes(startNode, firstColumnNodes);
        Debug.Log("노드 생성 완료");

        ConnectNodes(bossNode);
    }

    NodeType DetermineNodeType(int x, int y)
    {
        if (x == 0) return NodeType.Battle_Normal; // 첫 열은 일반 배틀
        if (x == 4) return NodeType.Treasure; // 5열은 트레저
        if (x == 9) return NodeType.Comma; // 10열은 콤마
        if (x > 9) return NodeType.Boss; // 10열 이후는 보스

        float rand = Random.value;

        if (rand < battleNormalProbability && x <= 4) return NodeType.Battle_Normal; // 일반 배틀 제한
        rand -= battleNormalProbability;

        if (rand < battleEliteProbability && x >= 6) return NodeType.Battle_Elite; // 정예 배틀 제한
        rand -= battleEliteProbability;

        if (rand < questProbability) return NodeType.Quest;
        rand -= questProbability;

        if (rand < marketProbability) return NodeType.Market;
        rand -= marketProbability;

        return NodeType.Comma; // 기본값
    }

    void ConnectNodes(Node bossNode)
    {
        HashSet<Node> activeNodes = new HashSet<Node>();

        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null)
            {
                activeNodes.Add(nodes[0, y]);
            }
        }

        for (int x = 0; x < columns - 1; x++) // 마지막 열 제외
        {
            HashSet<Node> nextActiveNodes = new HashSet<Node>();
            foreach (Node currentNode in activeNodes)
            {
                List<Node> possibleNeighbors = new List<Node>();

                // 다음 열의 가능한 노드 수집
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int nextY = (int)(currentNode.Position.y / nodeSpacing) + offsetY;
                    if (nextY >= 0 && nextY < rows && nodes[x + 1, nextY] != null)
                    {
                        possibleNeighbors.Add(nodes[x + 1, nextY]);
                    }
                }

                // 무작위로 최대 2개의 이웃 노드 선택
                int maxConnections = Random.Range(1, 3); // 1 또는 2
                while (possibleNeighbors.Count > 0 && currentNode.Neighbors.Count < maxConnections)
                {
                    Node nextNode = possibleNeighbors[Random.Range(0, possibleNeighbors.Count)];
                    possibleNeighbors.Remove(nextNode);

                    // 경로 추가
                    float distance = Vector2.Distance(currentNode.Position, nextNode.Position);
                    distance *= Random.Range(0.9f, 1.1f); // 랜덤 오차 적용

                    currentNode.Neighbors.Add(nextNode);
                    edges.Add((currentNode, nextNode, distance));

                    nextActiveNodes.Add(nextNode);
                }
            }

            // 다음 열의 활성 노드만 유지
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x + 1, y];
                if (node != null && !nextActiveNodes.Contains(node))
                {
                    nodes[x + 1, y] = null;
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
            Gizmos.DrawLine(edges[i].Item1.Position, edges[i].Item2.Position);
        }

        // 노드 그리기
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x, y];
                if (node != null)
                {
                    Gizmos.color = GetNodeColor(node.Type);
                    Gizmos.DrawSphere(node.Position, 0.2f);
                }
            }
        }

        // 시작 노드와 보스 노드 표시
        Node startNode = new Node(new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startNode.Position, 0.3f);

        Node bossNode = new Node(new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(bossNode.Position, 0.3f);
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
            float distance = Vector2.Distance(startNode.Position, selectedNode.Position);
            startNode.Neighbors.Add(selectedNode);
            edges.Add((startNode, selectedNode, distance));
        }

        // 1열에서 선택되지 않은 노드 제거
        foreach (Node node in firstColumnNodes)
        {
            for (int y = 0; y < rows; y++)
            {
                if (nodes[0, y] == node) // 정확히 매칭되는 노드만 제거
                {
                    nodes[0, y] = null;
                    Debug.Log($"Removed unselected node at position: {node.Position}");
                    break;
                }
            }
        }
    }

    void HandleFinalBossNode(Node bossNode)
    {
        // 10열 노드 수집
        List<Node> lastColumnNodes = new List<Node>();

        for (int y = 0; y < rows; y++)
        {
            Node node = nodes[columns - 1, y];
            if (node != null) lastColumnNodes.Add(node);
        }

        // 10열의 모든 노드를 보스전 노드와 연결
        foreach (Node node in lastColumnNodes)
        {
            float distance = Vector2.Distance(bossNode.Position, node.Position);
            node.Neighbors.Add(bossNode);
            edges.Add((node, bossNode, distance));
        }

        if (!IsGraphConnected())
        {
            Debug.LogError("그래프가 연결되어 있지 않습니다!");
        }
    }

    bool IsGraphConnected()
    {
        HashSet<Node> visited = new HashSet<Node>();
        Stack<Node> stack = new Stack<Node>();

        // 첫 번째 활성화된 노드 찾기
        Node startNode = null;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (nodes[x, y] != null)
                {
                    startNode = nodes[x, y];
                    break;
                }
            }
            if (startNode != null) break;
        }

        if (startNode == null)
        {
            Debug.LogError("No active nodes found. Cannot check connectivity.");
            return false;
        }

        stack.Push(startNode);

        // DFS 탐색
        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();

            if (currentNode != null && !visited.Contains(currentNode))
            {
                visited.Add(currentNode);
                Debug.Log($"Visited node at {currentNode.Position}");

                foreach (Node neighbor in currentNode.Neighbors)
                {
                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        // 연결된 노드 개수와 활성화된 노드 개수 비교
        foreach (Node node in nodes)
        {
            if (node != null)
            {
                Debug.Log(node.Position.ToString());
                if (node != null && !visited.Contains(node))
                {
                    Debug.LogError($"Node at position {node.Position} is not connected.");
                    return false; // 방문하지 않은 노드가 있으면 연결되지 않은 그래프
                }
            }
        }

        return true; // 모든 노드가 연결되어 있음
    }
}