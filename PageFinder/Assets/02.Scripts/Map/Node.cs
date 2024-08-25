using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MapType
{
    smallMap,
    mediumMap,
    largeMap
};

public class Node : MonoBehaviour
{

    private MapType mapType;

    private Node left;
    private Node right;
    private Node up;
    private Node down;
    private int connectCount;

    private Vector3 leftCorridorPosition;
    private Vector3 rightCorridorPosition;
    private Vector3 topCorridorPosition;
    private Vector3 bottomCorridorPosition;

    private bool isLeftConnected;
    private bool isRightConnected;
    private bool isTopConnected;
    private bool isBottomConnected;
   
    private Vector3 pos;
    private Tuple<int, int> index;

    private Transform tr;
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
    public Tuple<int, int> Index { get => index; 
        set 
        { 
            index = value;
        }  
    }

    public Vector3 LeftCorridorPosition 
    { get => leftCorridorPosition; set => leftCorridorPosition = value; }
    public Vector3 RightCorridorPosition 
    { get => rightCorridorPosition; set => rightCorridorPosition = value; }
    public Vector3 TopCorridorPosition 
    { get => topCorridorPosition; set => topCorridorPosition = value; }
    public Vector3 BottomCorridorPosition 
    { get => bottomCorridorPosition; set => bottomCorridorPosition = value; }

    public bool IsLeftConnected { get => isLeftConnected; set => isLeftConnected = value; }
    public bool IsRightConnected { get => isRightConnected; set => isRightConnected = value; }
    public bool IsTopConnected { get => isTopConnected; set => isTopConnected = value; }
    public bool IsBottomConnected { get => isBottomConnected; set => isBottomConnected = value; }

    public void Awake()
    {
        tr = GetComponent<Transform>();
    }

    public void Initialize(Vector3 pos, Tuple<int, int> index, MapType mapType, Node left = null, Node right = null, Node up = null, Node down = null,
        int connectCount = 0)
    {
        this.pos = pos;
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;
        this.connectCount = connectCount;
        this.index = index;
        this.mapType = mapType;
        isLeftConnected = false;
        isRightConnected = false;
        isTopConnected = false;
        isBottomConnected = false;
    }

    public void SetCorridor()
    {
        switch(mapType){
            case MapType.smallMap:
                SetCorridorPos(new Tuple<int, int>(20, 20));
                break;
            case MapType.mediumMap:
                SetCorridorPos(new Tuple<int, int>(24, 20));
                break;
            case MapType.largeMap:
                SetCorridorPos(new Tuple<int, int>(32, 24));
                break;
        }

    }

    public void SetCorridorPos(Tuple<int, int> mapSize)
    {
        leftCorridorPosition = new Vector3(tr.position.x - mapSize.Item1 / 2 - 0.5f, 0, tr.position.z);
        rightCorridorPosition = new Vector3(tr.position.x + mapSize.Item1 / 2 + 0.5f, 0, tr.position.z );
        topCorridorPosition = new Vector3(tr.position.x, 0, tr.position.z + mapSize.Item2 / 2 + 0.5f);
        bottomCorridorPosition = new Vector3(tr.position.x, 0, tr.position.z - mapSize.Item2 / 2 - 0.5f);
    }
}
