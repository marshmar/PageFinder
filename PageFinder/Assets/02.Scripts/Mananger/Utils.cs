using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// �� ������ ���� ���� ����
    /// </summary>
    /// <param name="a">���� 1�� ���� ����</param>
    /// <param name="b">���� 1�� �� ����</param>
    /// <param name="c">���� 2�� ���� ����</param>
    /// <param name="d">���� 2�� �� ����</param>
    /// <returns>���� 1�� 2�� ���� ����</returns>
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
    /// �׷��� ���� ���� ����
    /// </summary>
    /// <param name="rootNode">���� ���</param>
    /// <param name="nodes">��ü ��� ����Ʈ</param>
    /// <returns>�׷��� ���� ����</returns>
    public static bool IsGraphConnected(Node rootNode, Node[,] nodes)
    {
        HashSet<Node> visited = new();
        Stack<Node> stack = new();

        // DFS Ž��
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

        // Ȱ��ȭ�� ��� ��� �湮 ���� ��Ȯ��
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