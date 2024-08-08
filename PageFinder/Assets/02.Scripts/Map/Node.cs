using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Node left;
    private Node right;
    private Node up;
    private Node down;
    private int connectCount;

    private Vector3 pos;

    public Vector3 Pos { get => pos; set => pos = value; }

    public Node Left { get => left; set => left = value; }
    public Node Right { get => right; set => right = value; }
    public Node Up { get => up; set => up = value; }
    public Node Down { get => down; set => down = value; }

    public int ConnectCount { get => connectCount; set => connectCount = value; }

    public Node(Vector3 pos, Node left = null, Node right = null, Node up = null, Node down = null, int connectCount = 0)
    {
        this.pos = pos;
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;
        this.connectCount = connectCount;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
