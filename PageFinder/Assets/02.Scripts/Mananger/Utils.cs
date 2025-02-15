using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// 두 선분의 교차 여부 검증
    /// </summary>
    /// <param name="a">선분 1의 시작 정점</param>
    /// <param name="b">선분 1의 끝 정점</param>
    /// <param name="c">선분 2의 시작 정점</param>
    /// <param name="d">선분 2의 끝 정점</param>
    /// <returns>선분 1과 2의 교차 여부</returns>
    public static bool IsCrossing(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float CCW(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
        }

        return (CCW(a, b, c) * CCW(a, b, d) < 0) &&
               (CCW(c, d, a) * CCW(c, d, b) < 0);
    }

    /// <summary>
    /// 그래프 연결 상태 검증
    /// </summary>
    /// <param name="rootNode">시작 노드</param>
    /// <param name="nodes">전체 노드 리스트</param>
    /// <returns>그래프 연결 여부</returns>
    public static bool IsGraphConnected(Node rootNode, Node[,] nodes)
    {
        HashSet<Node> visited = new();
        Stack<Node> stack = new();

        // DFS 탐색
        stack.Push(rootNode);
        visited.Add(rootNode);

        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();

            foreach (int neighborID in currentNode.neighborIDs)
            {
                Node neighbor = NodeManager.Instance.GetNodeByID(neighborID);
                if (neighbor is not null && !visited.Contains(neighbor))
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
}