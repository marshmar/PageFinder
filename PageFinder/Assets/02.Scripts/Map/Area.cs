using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Area
{
    private /*Tuple<Node, Vector3>*/Node[,] area;
    private LinkedList<Node> mapList;
    private Tuple<int, int> startIndex;
    private int dim;

    private int[] dy;
    private int[] dx;

    public Tuple<int, int> StartIndex { get => startIndex; set => startIndex = value; }
    public int Dim { get => dim; set => dim = value; }

    Queue<Tuple<int, int>> bfsQueue;

    public Area(int dim, Tuple<int, int> startIndex, Node startNode)
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
        area = new Node[dim, dim];
        area[startIndex.Item1, startIndex.Item2] = startNode;
        mapList = new LinkedList<Node>();
        mapList.AddFirst(startNode);
        bfsQueue = new Queue<Tuple<int, int>>();
    }

    public bool BFS(Tuple<int, int> pos)
    {
        bfsQueue.Enqueue(pos);
        
        while(bfsQueue.Count != 0)
        {
            Tuple<int, int> p = bfsQueue.Dequeue();

            if (area[p.Item1, p.Item2] == null) continue;
            
        }
        return true;
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
        return area[index.Item1, index.Item2];
    }

    public Node[,] GetArea()
    {
        return area;
    }

    public void SetArea(Tuple<int, int> index, Node node)
    {
        area[index.Item1, index.Item2] = node;
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
