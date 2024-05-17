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
    /// ���� ����� �Ÿ��� ������Ʈ ���� ���� ������ ã��
    /// </summary>
    /// <param name="originPos">Ž�� ���� ����</param>
    /// <param name="searchDistance">Ž�� �Ÿ�</param>
    /// <param name="layer">Ž���� ���̾�</param>
    /// <returns>���� ����� �Ÿ��� ������Ʈ</returns>
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
    /// <summary>
    /// Collider ����Ʈ ������ ���� ����� �� ã��
    /// </summary>
    /// <param name="originPos">����</param>
    /// <param name="attackDist">Ž�� �Ÿ�</param>
    /// <param name="objects">Ž���� ����Ʈ</param>
    /// <returns>���� ����� ��ü�� Collider</returns>
    public Collider FindMinDistanceObject(Vector3 originPos, List<Collider> objects)
    {
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
}