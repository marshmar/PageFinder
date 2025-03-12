using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    private static Collider minDistObject;
    private static Collider[] objects;
    /// <summary>
    /// Check if two line segments intersect
    /// </summary>
    /// <param name="a">Starting vertex of line segment 1</param>
    /// <param name="b">End vertex of line segment 1</param>
    /// <param name="c">Starting vertex of line segment 2</param>
    /// <param name="d">End vertex of line segment 2</param>
    /// <returns>Whether line segments 1 and 2 intersect</returns>
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
    /// Verify graph connection status
    /// </summary>
    /// <param name="rootNode">Starting Node</param>
    /// <param name="nodes">Full Node List</param>
    /// <returns>Whether the graph is connected or not</returns>
    public static bool IsGraphConnected(Node rootNode, Node[,] nodes)
    {
        HashSet<Node> visited = new();
        Stack<Node> stack = new();

        // DFS Navigation
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

        // Recheck if all active nodes have been visited
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

    /// <summary>
    /// 가장 가까운 거리의 오브젝트 구형 범위 내에서 찾기
    /// </summary>
    /// <param name="originPos">탐색 시작 지점</param>
    /// <param name="searchDistance">탐색 거리</param>
    /// <param name="layer">탐색할 레이어</param>
    /// <returns>가장 가까운 거리의 오브젝트</returns>
    public static Collider FindMinDistanceObject(Vector3 originPos, float searchDistance, int layer)
    {
        float minDist = searchDistance;
        minDistObject = null;
        //objects = Physics.OverlapBox(new Vector3(originPos.x, 0.0f, originPos.z), new Vector3(searchDistance, 10.0f, searchDistance), Quaternion.identity, layer);
        objects = Physics.OverlapSphere(originPos, searchDistance * 2.0f, layer);

        foreach (Collider i in objects)
        {
            Vector2 coll = new Vector2(i.gameObject.transform.position.x, i.gameObject.transform.position.z);
            Vector2 newOriPos = new Vector2(originPos.x, originPos.z);
            float dist = Vector2.Distance(newOriPos, coll);
            if (minDist >= dist)
            {
                minDistObject = i;
                minDist = dist;
            }
        }
        return minDistObject;
    }

    /// <summary>
    /// Collider 리스트 내에서 가장 가까운 적 찾기
    /// </summary>
    /// <param name="originPos">원점</param>
    /// <param name="atkDist">탐색 거리</param>
    /// <param name="objects">탐색할 리스트</param>
    /// <returns>가장 가까운 객체의 Collider</returns>
    public static Collider FindMinDistanceObject(Vector3 originPos, List<Collider> objects)
    {
        if (objects[0] == null) return null;
        float minDist = Vector3.Distance(originPos, objects[0].transform.position);
        minDistObject = null;
        for (int i = 0; i < objects.Count; i++)
        {
            float dist = Vector3.Distance(originPos, objects[i].gameObject.transform.position);
            Debug.Log(objects[i].gameObject.name + dist);
            if (minDist >= dist)
            {
                minDistObject = objects[i];
                minDist = dist;
            }
        }
        return minDistObject;
    }
}