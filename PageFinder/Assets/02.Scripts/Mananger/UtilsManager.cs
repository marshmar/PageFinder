using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsManager :  Singleton<UtilsManager>
{
    Collider minDistObject;
    Collider[] objects;

    private void Start()
    {
        
    }
    /// <summary>
    /// 가장 가까운 거리의 오브젝트 구형 범위 내에서 찾기
    /// </summary>
    /// <param name="originPos">탐색 시작 지점</param>
    /// <param name="searchDistance">탐색 거리</param>
    /// <param name="layer">탐색할 레이어</param>
    /// <returns>가장 가까운 거리의 오브젝트</returns>
    public Collider FindMinDistanceObject(Vector3 originPos, float searchDistance, int layer)
    {
        float minDist = searchDistance;
        minDistObject = null;
        objects = Physics.OverlapSphere(originPos, searchDistance, layer);
        foreach (Collider i in objects)
        {
            float dist = Vector3.Distance(originPos, i.gameObject.transform.position);
            if (minDist >= dist)
            {
                minDistObject = i;
                minDist = dist;
            }
        }
        return minDistObject;
    }
}
