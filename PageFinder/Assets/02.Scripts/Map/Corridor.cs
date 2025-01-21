using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    public GameObject bridgeSegment;  // 다리 조각 프리팹
    public Transform pointA;          // 시작 지점
    public Transform pointB;          // 끝 지점
    public float segmentLength = 1.0f; // 각 다리 조각의 길이

    void Start()
    {
        CreateBridge(pointA.position, pointB.position);
    }

    void CreateBridge(Vector3 start, Vector3 end)
    {
        // 두 지점 사이의 벡터 계산
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        direction.Normalize();

        // 다리 조각의 개수 계산
        int segmentCount = Mathf.CeilToInt(distance / segmentLength);

        for (int i = 0; i <= segmentCount; i++)
        {
            // 다리 조각의 위치 계산
            Vector3 segmentPosition = start + direction * segmentLength * i;

            // 다리 조각 생성
            GameObject segment = Instantiate(bridgeSegment, segmentPosition, Quaternion.LookRotation(direction));
            segment.GetComponent<Transform>().Rotate(new Vector3(90, 0, 0));
        }
    }
}
