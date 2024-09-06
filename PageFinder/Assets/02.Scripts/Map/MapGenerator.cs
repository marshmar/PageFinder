using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public GameObject mapObj;
    public GameObject largeMap;
    public GameObject mediumMap;
    public GameObject smallMap;
    public GameObject bridgeSegment;  // 다리 조각 프리팹

    [Range(400, 500)]
    public int radius;
    [Range(1, 6)]
    public int placeCount;
    [Range(1, 20)]
    public int roomCount;

    // areaLength는 짝수여야함.
    [Range(100,300)]
    public int areaLength;


    private Node newNode;

    public float segmentLength = 1.0f; // 각 다리 조각의 길이
    /*    private enum MapType
        {
            MiddleBoss,
            Token,
            Object
        }*/

    private Tuple<int, int>[] directions;
    private Tuple<int, int> currDir;
    private List<Area> mapAreas;
    private GameObject[] mapAreaObjs;

    private MapType currMapType;

    // Start is called before the first frame update
    void Start()
    {
        mapAreaObjs = new GameObject[placeCount];
        mapAreas = new List<Area>();
        directions = new Tuple<int, int>[4]{
            new Tuple<int, int>(0, -1), // left
            new Tuple<int, int>(0, 1), // right
            new Tuple<int, int>(1, 0), // up
            new Tuple<int, int>(-1, 0)  // down
        };

        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 전체적인 맵 개발 프로세스
    /// 맵은 2단계 과정을 거쳐 생성함
    /// </summary>
    public void CreateMap()
    {
        CreateArea();
        CreateNodeInArea();
    }

    /// <summary>
    /// 시작점을 결정하는 함수 
    /// 반지름이 radius인 원에서 출발하여 내접하는 정다각형을 그려서 시작점을 결정
    /// </summary>
    private void CreateArea()
    {
        // 라디안값으로 변환
        float angleInRandian = 2.0f * MathF.PI / placeCount;

        for(int i = 0; i < placeCount; i++)
        {
            // 정다각형의 꼭짓점 생성
            Vector3 pos = new Vector3(
                (int)(MathF.Cos(angleInRandian * i) * radius),
                0,
                (int)(MathF.Sin(angleInRandian * i) * radius)
                );

            int dim = 5;
            Tuple<int, int> startIndex = new Tuple<int, int>((UnityEngine.Random.Range(0, dim)), (UnityEngine.Random.Range(0, dim)));
            mapAreaObjs[i] = new GameObject($"Area {i}");
            Area area = new Area(5, startIndex, pos);

            GameObject newObj = Instantiate(largeMap, area.GetNodesLocalPosition(startIndex), Quaternion.identity);
            Transform newObjTr = newObj.GetComponent<Transform>();
            currMapType = MapType.largeMap;
            newObjTr.Rotate(new Vector3(0, 90, 0));
            newObjTr.parent = mapAreaObjs[i].GetComponent<Transform>();
            newObj.name = "Start";

            newNode = newObj.AddComponent<Node>();
            newNode.Initialize(area.GetNodesLocalPosition(startIndex), startIndex, currMapType);
            newNode.SetCorridor();

            area.SetNode(startIndex, newNode);
            area.AddNode(newNode);
            mapAreas.Add(area);

        }
    }

    private void CreateNodeInArea()
    {
        for(int i = 0; i < mapAreas.Count; i++)
        {
            CreateNode(mapAreas[i], mapAreas[i].StartIndex, roomCount, mapAreaObjs[i].GetComponent<Transform>());
            CreateBridgeMaximumNode(mapAreas[i], mapAreaObjs[i]);
        }
    }

    private void CreateBridgeMaximumNode(Area currArea, GameObject currAreaObj)
    {
        Node maximumNode = FindMaximumLinkedNode(currArea);
        if(maximumNode == null)
        {
            Debug.LogError("maximumNode is not found");
        }
        CreateCorridorForEachDirection(currAreaObj.GetComponent<Transform>(), maximumNode);
    }

    private void CreateCorridorForEachDirection(Transform currAreaTr, Node maximumNode)
    {
        GameObject corridorObj = new GameObject($"Bridge From {maximumNode.name} ");
        Transform corridorObjTr = corridorObj.GetComponent<Transform>();
        corridorObjTr.parent = currAreaTr;
        if (maximumNode.Up != null && !maximumNode.IsTopConnected)
        {
            CreateCorridor(maximumNode.BottomCorridorPosition, maximumNode.Up.TopCorridorPosition, corridorObjTr);
        }
        if (maximumNode.Down != null && !maximumNode.IsBottomConnected)
        {
            CreateCorridor(maximumNode.TopCorridorPosition, maximumNode.Down.BottomCorridorPosition, corridorObjTr);
        }
        if (maximumNode.Left != null && !maximumNode.IsLeftConnected)
        {
            CreateCorridor(maximumNode.LeftCorridorPosition, maximumNode.Left.RightCorridorPosition, corridorObjTr);
        }
        if (maximumNode.Right != null && !maximumNode.IsRightConnected)
        {
            CreateCorridor(maximumNode.RightCorridorPosition, maximumNode.Right.LeftCorridorPosition, corridorObjTr);
        }
    }

    private void CreateNode(Area currArea, Tuple<int, int> before, int roomCount, Transform parentArea)
    {
        if (roomCount == 0)
            return;
        else
        {
            Tuple<int, int> curr;
            while (true) 
            {
                SetDirection();
                curr = new Tuple<int, int>(currDir.Item1 + before.Item1, currDir.Item2 + before.Item2);
                
                if (currArea.CheckNodeInArea(curr))
                {
                    if (currArea.GetNodeInArea(curr) == null)
                    {
                        // 현재 지정한 방향을 기점으로 bfs 돌리기
                        int currDirectionLeftAreaCount = currArea.BFS(curr);
                        // 만약 현재 지정한 방향에서 bfs를 돌렸을 때 만들 수 있는 방의 개수가
                        // 만들어야 하는 방의 개수보다 작거나 같으면
                        // 나머지 방향중에서 다시 설정하기
                        if (currDirectionLeftAreaCount >= roomCount)
                        {

                            GameObject newObj = Instantiate(ChooseRandomMap(), currArea.GetNodesLocalPosition(curr), Quaternion.identity);
                            Transform newObjTr = newObj.GetComponent<Transform>();

                            newObj.name = $"{Mathf.Abs(roomCount - this.roomCount)+1}";
                            if (roomCount == 1)
                                newObj.name = "End";

                            newNode = newObj.AddComponent<Node>();
                            newNode.Initialize(currArea.GetNodesLocalPosition(curr), curr, currMapType);
                            newNode.SetCorridor();

                            newObjTr.Rotate(new Vector3(0, 90, 0));
                            newObjTr.parent = parentArea;

                            GameObject corridorObj = new GameObject($"Bridge To {newObj.name}");
                            Transform corridorObjTr = corridorObj.GetComponent<Transform>();
                            corridorObjTr.parent = parentArea;
                            CheckDirectionAndCreateCorridor(currArea, newNode, corridorObjTr);

                            currArea.SetNode(curr, newNode);
                            currArea.ConnectNode(curr, newNode);
                            currArea.AddNode(newNode);
                            break;
                        }
                    }
                }
            }

            CreateNode(currArea, curr, --roomCount, parentArea);
        }
    }

    private void CheckDirectionAndCreateCorridor(Area currArea, Node newNode, Transform parentCorridor)
    {
        Node beforeNode = currArea.GetMapList().Last.Value;
        if(currDir == directions[0]) // left
        {
            beforeNode.IsLeftConnected = true;
            newNode.IsRightConnected = true;
            CreateCorridor(beforeNode.LeftCorridorPosition, newNode.RightCorridorPosition, parentCorridor);
        }
        else if(currDir == directions[1]) // right
        {
            beforeNode.IsRightConnected = true;
            newNode.IsLeftConnected = true;
            CreateCorridor(beforeNode.RightCorridorPosition, newNode.LeftCorridorPosition, parentCorridor);
        }
        else if(currDir == directions[2]) // up
        {
            beforeNode.IsBottomConnected = true;
            newNode.IsTopConnected = true;
            CreateCorridor(beforeNode.TopCorridorPosition, newNode.BottomCorridorPosition, parentCorridor);
        }
        else                              // bottom
        {
            beforeNode.IsTopConnected = true;
            newNode.IsBottomConnected = true;
            CreateCorridor(beforeNode.BottomCorridorPosition, newNode.TopCorridorPosition, parentCorridor);
        }
    }

    private GameObject ChooseRandomMap()
    {
        GameObject randomMap = null;
        int randomSize = UnityEngine.Random.Range(0, 3);
        switch (randomSize)
        {
            case 0:
                currMapType = MapType.smallMap;
                randomMap = smallMap;
                break;
            case 1:
                randomMap = mediumMap;
                currMapType = MapType.mediumMap;
                break;
            case 2:
                currMapType = MapType.largeMap;
                randomMap = largeMap;
                break;
        }
        return randomMap;
    }

    private int SetDirection()
    {
        int dir = UnityEngine.Random.Range(0, 4);
        currDir = directions[dir];
        return dir;
    }

    private void CreateCorridor(Vector3 start, Vector3 end, Transform parentCorridor)
    {
        // 두 지점 사이의 벡터 계산
        Vector3 direction = end - start;
        int distance = (int)direction.magnitude;
        direction.Normalize();

        // 다리 조각의 개수 계산
        int segmentCount = Mathf.CeilToInt(distance / segmentLength);

        for (int i = 0; i <= segmentCount; i++)
        {
            // 다리 조각의 위치 계산
            Vector3 segmentPosition = start + direction * segmentLength * i;

            // 다리 조각 생성
            GameObject segment = Instantiate(bridgeSegment, segmentPosition, Quaternion.LookRotation(direction));
            Transform segmentTr = segment.GetComponent<Transform>();
            segmentTr.Rotate(new Vector3(90, 0, 0));
            segmentTr.parent = parentCorridor;
        }
    }

    // 가장 많이 연결된 맵을 찾는 함수
    private Node FindMaximumLinkedNode(Area currArea)
    {
        LinkedList<Node> mapList = currArea.GetMapList();
        Node maximumLinkedNode = mapList.First.Value;
        foreach(Node node in mapList)
        {
            if (node.ConnectCount > maximumLinkedNode.ConnectCount)
                maximumLinkedNode = node;
        }

        return maximumLinkedNode;
    }
}
