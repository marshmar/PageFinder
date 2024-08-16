using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Node left;
    private Node right;
    private Node up;
    private Node down;
    private int connectCount;

    private Vector3 pos;

    public Vector3 Pos { get => pos; set => pos = value; }

    public Node Left { get => left;
        set { 
            left = value;
            connectCount++;
        }
    }
    public Node Right { get => right;
        set
        { 
            right = value;
            connectCount++;
        }
    }

    public Node Up { 
        get => up;
        set { 
            up = value;
            connectCount++;
        }
    }
    public Node Down
    {
        get => down;
        set
        {
            down = value;
            connectCount++;
        }
    }

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

}
