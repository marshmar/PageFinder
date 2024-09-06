using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class RangedEntity : MonoBehaviour
{
    private Transform tr;
    private LineRenderer lineRenderer;
    public GameObject circleObjPrefab;
    private GameObject circleObj;
    // Start is called before the first frame update
    void Start()
    {
        Hashing();
        lineRenderer = GetComponent<LineRenderer>();
        circleObj = Instantiate(circleObjPrefab, tr.position, Quaternion.identity);
        circleObj.transform.Rotate(90, 0, 0);
        circleObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hashing()
    {
        tr = GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetPositionsForLine(Vector3 startPos, Vector3 endPos)
    {
        SetLineRenderePositionCounts(2);
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public void SetPoisitonsForCircle(Vector3 center, float radius)
    {
        circleObj.transform.localScale = new Vector3(radius, radius, radius);
        circleObj.transform.position = new Vector3(center.x, 0.1f, center.z);
/*        SetLineRenderePositionCounts(360);
        const float radian = (Mathf.PI * 2) / 360;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        Debug.Log(radius);
        for(int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = radian * i;

            Vector3 point = center;
            point.x = point.x + Mathf.Cos(angle) * radius;
            point.z = point.z + Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(i, point);
        }*/
    }

    private void SetLineRenderePositionCounts(int positionCounts)
    {
        lineRenderer.positionCount = positionCounts;
    }

    public void EnableLineRenderer()
    {
        lineRenderer.enabled = true;
    }

    public void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }

    public void EnableCircleRenderer()
    {
        circleObj.SetActive(true);
    }

    public void DisableCircleRenderer()
    {
        circleObj.SetActive(false);
    }
}
