using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public GameObject mapObj;
    public GameObject TestObj;

    [Range(100, 200)]
    public int radius;
    [Range(1, 6)]
    public int placeCount;
    [Range(1, 6)]
    public int roomCount;

    // areaLength는 짝수여야함.
    [Range(100,300)]
    public int areaLength;

    private int StartPosWidth;
    private int StartPosHeight;


    private GameObject[] pivots;
    private int randomOffset;
    private Vector3 topLeft;
    private Vector3 topRight;
    private Vector3 bottomLeft;
    private Vector3 bottomRight;

    private LineRenderer lineRenderer;

    float randomAngle;

    private enum CoordinateCondition
    {
        PositiveXZ,             // x > StartPosWidth, z > StartPosHeight
        PositiveXNegativeZ,     // x > StartPosWidth, z < StartPosHeight
        NegativeXZ,             // x < StartPosWidth, z < StartPosHeight
        NegativeXPositiveZ,     // x < StartPosWidth, z > StartPosHeight
        ZeroXPositiveZ,         // x ∈ StartPosWidth, z > StartPosHeight
        ZeroXNegativeZ,         // x ∈ StartPosWidth, z < StartPosHeight
        PositiveXZeroZ,         // x > StartPosWidth, z ∈ StartPosHeight
        NegativeXZeroZ          // x < StartPosWidth, z ∈ StartPosHeight
    }

    private CoordinateCondition posCondition;
    private List<List<Vector3>> mapPos;

    // Start is called before the first frame update
    void Start()
    {
        pivots = new GameObject[placeCount];
        mapPos = new List<List<Vector3>>();
        StartPosWidth = 10;
        StartPosHeight = 10;
        randomOffset = 20;
        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMap()
    {
        CreateStartPos();
        CreateAreaMap();
    }

    /// <summary>
    /// 시작점을 결정하는 함수 
    /// 반지름이 radius인 원에서 출발하여 내접하는 정다각형을 그려서 시작점을 결정
    /// </summary>
    private void CreateStartPos()
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

            mapPos.Add(new List<Vector3>());
            mapPos[i].Add(pos);
            pivots[i] = Instantiate(TestObj, pos, Quaternion.identity);
            pivots[i].AddComponent<LineRenderer>();
        }
    }

    private void CreateAreaMap()
    {
        for(int i = 0; i < mapPos.Count; i++)
        {
            lineRenderer = pivots[i].GetComponent<LineRenderer>();
            lineRenderer.startWidth = .5f;
            lineRenderer.endWidth = .5f;
            foreach (Vector3 position in mapPos[i])
            {
                lineRenderer.positionCount = 5;
                DefinePosCondition(position);
                DefineVirtualSqaure(position);
                CreateSqaure();
                CreateArea(position, roomCount);
            }
        }

    }

    private void CreateArea(Vector3 pos, int roomCount)
    {
        if (roomCount == 0)
            return;
        else
        {
            Vector3 areaPos = pos + new Vector3(UnityEngine.Random.Range(0, randomOffset),0, UnityEngine.Random.Range(0, randomOffset));
            while (CheckPosiotionInSqaure(areaPos) == false)
            {
                areaPos = pos + new Vector3(UnityEngine.Random.Range(0, randomOffset),0, UnityEngine.Random.Range(0, randomOffset));
            }
            Instantiate(TestObj, areaPos, Quaternion.identity);
            CreateArea(areaPos, --roomCount);
        }
    }

    private bool CheckPosiotionInSqaure(Vector3 areaPos)
    { 
        return true;
    }

    private void DefineVirtualSqaure(Vector3 pos)
    {
        switch (posCondition)
        {
            case CoordinateCondition.PositiveXZ:
                topLeft = pos + new Vector3(0, 0, areaLength);
                topRight = pos + new Vector3(areaLength, 0, areaLength);
                bottomLeft = pos;
                bottomRight = pos + new Vector3(areaLength, 0, 0);
                break;
            case CoordinateCondition.PositiveXNegativeZ:
                topLeft = pos;
                topRight = pos + new Vector3(areaLength, 0, 0);
                bottomLeft = pos + new Vector3(0, 0, -areaLength);
                bottomRight = pos + new Vector3(areaLength, 0, -areaLength);
                break;
            case CoordinateCondition.NegativeXPositiveZ:
                topLeft = pos + new Vector3(-areaLength, 0, areaLength);
                topRight = pos + new Vector3(0, 0, areaLength);
                bottomLeft = pos + new Vector3(-areaLength, 0, 0);
                bottomRight = pos;
                break;
            case CoordinateCondition.NegativeXZ:
                topLeft = pos + new Vector3(-areaLength, 0, 0);
                topRight = pos;
                bottomLeft = pos + new Vector3(-areaLength, 0, -areaLength);
                bottomRight = pos + new Vector3(0, 0, -areaLength);
                break;
            case CoordinateCondition.ZeroXPositiveZ:
                topLeft = pos + new Vector3(-areaLength / 2, 0, areaLength);
                topRight = pos + new Vector3(areaLength / 2, 0, areaLength);
                bottomLeft = pos + new Vector3(-areaLength / 2, 0, 0);
                bottomRight = pos + new Vector3(areaLength / 2, 0, 0);
                break;
            case CoordinateCondition.ZeroXNegativeZ:
                topLeft = pos + new Vector3(-areaLength / 2, 0, 0);
                topRight = pos + new Vector3(areaLength / 2, 0, 0);
                bottomLeft = pos + new Vector3(-areaLength / 2, 0, -areaLength);
                bottomRight = pos + new Vector3(areaLength / 2, 0, -areaLength);
                break;
            case CoordinateCondition.PositiveXZeroZ:
                topLeft = pos + new Vector3(0, 0, areaLength/2);
                topRight = pos + new Vector3(areaLength, 0, areaLength/2);
                bottomLeft = pos + new Vector3(0, 0, -areaLength/2);
                bottomRight = pos + new Vector3(areaLength, 0, -areaLength/2);
                break;
            case CoordinateCondition.NegativeXZeroZ:
                topLeft = pos + new Vector3(-areaLength, 0, areaLength / 2);
                topRight = pos + new Vector3(0, 0, areaLength / 2);
                bottomLeft = pos + new Vector3(-areaLength, 0, -areaLength / 2);
                bottomRight = pos + new Vector3(0, 0, -areaLength / 2);
                break;
        }
    }

    private void CreateSqaure()
    {
        lineRenderer.SetPosition(0, topLeft);
        lineRenderer.SetPosition(1, topRight);
        lineRenderer.SetPosition(2, bottomRight);
        lineRenderer.SetPosition(3, bottomLeft);
        lineRenderer.SetPosition(4, topLeft);
    }

    // 시작점 좌표 상태 설정하기
    private void DefinePosCondition(Vector3 pos)
    {
        if (pos.x > StartPosWidth && pos.z > StartPosHeight) posCondition = CoordinateCondition.PositiveXZ;
        else if (pos.x > StartPosWidth && pos.z < -StartPosHeight) posCondition = CoordinateCondition.PositiveXNegativeZ;
        else if (pos.x < -StartPosWidth && pos.z > StartPosHeight) posCondition = CoordinateCondition.NegativeXPositiveZ;
        else if (pos.x < -StartPosWidth && pos.z < -StartPosHeight) posCondition = CoordinateCondition.NegativeXZ;
        else if (pos.x <= StartPosWidth && pos.x >= -StartPosWidth && pos.z > StartPosHeight) posCondition = CoordinateCondition.ZeroXPositiveZ;
        else if (pos.x <= StartPosWidth && pos.x >= -StartPosWidth && pos.z < -StartPosHeight) posCondition = CoordinateCondition.ZeroXNegativeZ;
        else if (pos.x > StartPosWidth && pos.z <= StartPosHeight && pos.z >= -StartPosHeight) posCondition = CoordinateCondition.PositiveXZeroZ;
        else if (pos.x < -StartPosWidth && pos.z <= StartPosHeight && pos.z >= -StartPosHeight) posCondition = CoordinateCondition.NegativeXZeroZ;

/*        if (pos.x >= 0 && pos.z >= 0) posCondition = CoordinateCondition.PositiveXZ;
        else if (pos.x >= 0 && pos.z < 0) posCondition = CoordinateCondition.PositiveXNegativeZ;
        else if (pos.x < 0 && pos.z >= 0) posCondition = CoordinateCondition.NegativeXPositiveZ;
        else posCondition = CoordinateCondition.NegativeXZ;*/
    }
}
