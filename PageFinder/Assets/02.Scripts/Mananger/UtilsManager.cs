using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsManager :  Singleton<UtilsManager>, IListener
{
    Collider minDistObject;
    Collider[] objects;


    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.GAME_END, this);
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
        //objects = Physics.OverlapBox(new Vector3(originPos.x, 0.0f, originPos.z), new Vector3(searchDistance, 10.0f, searchDistance), Quaternion.identity, layer);
        objects = Physics.OverlapSphere(originPos, searchDistance * 2.0f, layer);

        foreach (Collider i in objects)
        {
            Vector2 coll = new Vector2(i.gameObject.transform.position.x, i.gameObject.transform.position.z);
            Vector2 newOriPos = new Vector2(originPos.x, originPos.z);
            float dist = Vector2.Distance(newOriPos, coll);
            if (minDist >= dist)
            {
                minDistObject = i;
                minDist = dist;
            }
        }
        return minDistObject;
    }

    /// <summary>
    /// Collider 리스트 내에서 가장 가까운 적 찾기
    /// </summary>
    /// <param name="originPos">원점</param>
    /// <param name="atkDist">탐색 거리</param>
    /// <param name="objects">탐색할 리스트</param>
    /// <returns>가장 가까운 객체의 Collider</returns>
    public Collider FindMinDistanceObject(Vector3 originPos, List<Collider> objects)
    {
        if (objects[0] == null) return null;
        float minDist = Vector3.Distance(originPos, objects[0].transform.position);
        minDistObject = null;
        for(int i = 0; i < objects.Count; i++){
            float dist = Vector3.Distance(originPos, objects[i].gameObject.transform.position);
            Debug.Log(objects[i].gameObject.name + dist);
            if (minDist >= dist)
            {
                minDistObject = objects[i];
                minDist = dist;
            }
        }
        return minDistObject;
    }
    
    public void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null)
    {
        switch (Event_Type)
        {
            case EVENT_TYPE.GAME_END:
                Destroy(this.gameObject);
                break;
        }
    }
}
