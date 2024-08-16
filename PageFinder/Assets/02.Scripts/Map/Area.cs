using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Area
{
    private Tuple<Node, Vector3>[,] area;
    private LinkedList<Node> mapList;
    private Tuple<int, int> startIndex;
    private int dim;
    private Vector3 centerPos;
    private int areaPositionOffset;

    private int[] dy;
    private int[] dx;

    public Tuple<int, int> StartIndex { get => startIndex; set => startIndex = value; }
    public int Dim { get => dim; set => dim = value; }
    public Vector3 CenterPos { get => centerPos; set => centerPos = value; }

    Queue<Tuple<int, int>> bfsQueue;

    public Area(int dim, Tuple<int, int> startIndex, Node startNode, Vector3 pos)//, Vector3 startPos)
    {
        dy = new int[4]{
            -1,
            1,
            0,
            0
        };
        dx = new int[4] { 
            0,
            0,
            -1,
            1
        };


        this.startIndex = startIndex;
        this.dim = dim;
        area = new Tuple<Node, Vector3>[dim, dim];
        centerPos = pos;

        SetLocationInArea(startNode);
        startNode.Pos = GetNodesLocalPosition(startIndex);

        mapList = new LinkedList<Node>();
        mapList.AddFirst(startNode);
        bfsQueue = new Queue<Tuple<int, int>>();
    }

    public void SetLocationInArea(Node startNode)
    {
        areaPositionOffset = 30;

        int centerX = dim / 2;
        int centerZ = dim / 2;

        area[centerX, centerZ] = new Tuple<Node, Vector3>(startNode, centerPos);

        for(int i = 0; i  < dim; i++)
        {
            for(int j = 0; j < dim; j++)
            {
                if(i != centerX || j != centerZ)
                {
                    int offsetX = i - centerX;
                    int offsetZ = j - centerZ;

                    Vector3 loaclPosition = centerPos + new Vector3(offsetX * areaPositionOffset, 0, offsetZ * areaPositionOffset);

                    area[i, j] = new Tuple<Node, Vector3>(null, loaclPosition);
                }
            }
        }
    }
    public int BFS(Tuple<int, int> pos)
    {
        int leftMapCount = 1;
        bfsQueue.Enqueue(pos);
        bool[,] visited = new bool[dim, dim];
        visited[pos.Item1, pos.Item2] = true;

        while (bfsQueue.Count != 0)
        {
            Tuple<int, int> p = bfsQueue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                if (!CheckNodeInArea(new Tuple<int, int>(p.Item1 + dy[i], p.Item2 + dx[i]))) continue;
                if (visited[p.Item1 + dy[i], p.Item2 + dx[i]]) continue;
                if (area[p.Item1 + dy[i], p.Item2 + dx[i]].Item1 != null) continue;

                visited[p.Item1 + dy[i], p.Item2 + dx[i]] = true;
                bfsQueue.Enqueue(new Tuple<int, int>(p.Item1 + dy[i], p.Item2 + dx[i]));
                leftMapCount++;
            }
        }
        return leftMapCount;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckNodeInArea(Tuple<int, int> index)
    {
        if (index.Item1 < 0 || index.Item1 >= dim) return false;
        if (index.Item2 < 0 || index.Item2 >= dim) return false;
        return true;
    }

    public Node GetNodeInArea(Tuple<int, int> index)
    {
        if (index.Item1 < 0 || index.Item1 >= dim) return null;
        if (index.Item2 < 0 || index.Item2 >= dim) return null;
        return area[index.Item1, index.Item2].Item1;
    }

    public Tuple<Node, Vector3>[,] GetArea()
    {
        return area;
    }

    public void SetNode(Tuple<int, int> index, Node node)
    {
        area[index.Item1, index.Item2] = new Tuple<Node, Vector3>(node, GetNodesLocalPosition(index));
    }

    public void ConnectNode(Tuple<int, int> index, Node currNode)
    {
        Node tempNode;
        tempNode = GetNodeInArea(new Tuple<int, int>(index.Item1 - 1, index.Item2)); // Up
        if (tempNode != null)
        {
            currNode.Up = tempNode;
            tempNode.Down = currNode;
        }
        tempNode = GetNodeInArea(new Tuple<int, int>(index.Item1 + 1, index.Item2)); // Down
        if(tempNode != null)
        {
            currNode.Down = tempNode;
            tempNode.Up = currNode;
        }
        tempNode = GetNodeInArea(new Tuple<int, int>(index.Item1, index.Item2 + 1)); // Right
        if(tempNode != null)
        {
            currNode.Right = tempNode;
            tempNode.Left = currNode;
        }
        tempNode = GetNodeInArea(new Tuple<int, int>(index.Item1, index.Item2 - 1)); // Left
        if(tempNode != null)
        {
            currNode.Left = tempNode;
            tempNode.Right = currNode;
        }
    }

    public Vector3 GetNodesLocalPosition(Tuple<int, int> index)
    {
        return area[index.Item1, index.Item2].Item2;
    }
    public void AddNode(Node node)
    {
        mapList.AddLast(node);
    }

    public LinkedList<Node> GetMapList()
    {
        return mapList;
    }
}
