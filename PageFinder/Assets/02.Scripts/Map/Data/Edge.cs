using System;
using UnityEngine;

[Serializable]
public class Edge
{
    public Node nodeA;
    public Node nodeB;
    public float distance;
    public GameObject LineUI;

    public Edge(Node nodeA, Node nodeB, float distance)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        this.distance = distance;
    }
}