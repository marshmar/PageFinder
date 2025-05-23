using System;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Unknown, Start, Battle_Normal, Battle_Elite, Battle_Elite1, Battle_Elite2, Quest, Treasure, Market, Comma, Boss }

[Serializable]
public class Node
{
    public int id { get; private set; }
    private static int nextID = 0;

    public int column;
    public int row;
    public Vector2 position;
    public NodeType type;
    public List<int> neighborIDs = new();
    public Node prevNode;
    public GameObject map;
    public GameObject ui;
    public Portal portal;

    public Node(int column, int row, Vector2 position, NodeType type, GameObject map)
    {
        id = nextID++;
        this.column = column;
        this.row = row;
        this.position = position;
        this.type = type;
        this.map = map;
    }
}