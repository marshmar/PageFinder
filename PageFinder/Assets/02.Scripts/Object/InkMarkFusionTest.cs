using System.Collections.Generic;
using UnityEngine;

public class InkMarkFusionTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float inkFusionCircleThreshold = 0.3f;
    private bool isAbleFusion = false;
    private int testNum = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            testNum = 0;
            Debug.Log("��, �� �ռ� ���");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            testNum = 1;
            Debug.Log("�簢��, �簢�� �ռ� ���");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            testNum = 2;
            Debug.Log("�簢��, �� �ռ� ���");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(testNum == 0)
        {
            if (CheckIntersectCircle(other))
            {
                isAbleFusion = true;
            }
            else
            {
                isAbleFusion = false;
            }
        }
        else if(testNum == 1)
        {

            if (CheckIntersectionRectangle(other))
            {
                isAbleFusion = true;
            }
            else
            {
                isAbleFusion = false;
            }
        }
        else
        {

            if (CheckIntersectBetweenRectangleCircle(other))
            {
                isAbleFusion = true;
            }
            else
            {
                isAbleFusion = false;
            }
        }




        if (isAbleFusion)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

    }

    //https://yupdown.tistory.com/31
    private bool CheckIntersectCircle(Collider other)
    {
        float r1 = transform.localScale.x * 0.5f;
        float r2 = other.transform.localScale.x * 0.5f;
        float distance = Vector3.Distance(transform.position, other.transform.position);
        if (r1 + r2 < distance) {
            Debug.Log("�� ���� ������ ����");
            return false;  // �� ���� � �κе� �������� �ʴ� ���
        }

        // ��ġ�� �κ��� �߽ɰ�
        float thetaR1 = 2 * Mathf.Acos((distance * distance + r1 * r1 - r2 * r2) / (2 * distance * r1));
        float thetaR2 = 2 * Mathf.Acos((distance * distance + r2 * r2 - r1 * r1) / (2 * distance * r2));

        // ��ġ�� �κ��� �ﰢ�� ����
        float triangleAreaR1 = r1 * r1 * 0.5f * Mathf.Sin(thetaR1);
        float triangleAreaR2 = r2 * r2 * 0.5f * Mathf.Sin(thetaR2);

        // ��ġ�� �κ��� ��ä���� ����
        float fanAreaR1 = r1 * r1 * 0.5f * thetaR1;
        float fanAreaR2 = r2 * r2 * 0.5f * thetaR2;

        // ���������� ��ġ�� �κ��� ����
        float intersectArea = fanAreaR1 - triangleAreaR1 + fanAreaR2 - triangleAreaR2;
        float circleArea = r1 * r1 * (float)Mathf.PI;

        //Debug.Log($"intersectArea: {intersectArea}, CircleArea: {circleArea}");

        if (intersectArea / circleArea >= inkFusionCircleThreshold) return true;

        return false;
    }

    Vector2[] basePointsTest;
    Vector2[] testPointsTest;

    // https://en.wikipedia.org/wiki/Sutherland%E2%80%93Hodgman_algorithm  ��������-������ Clipping �˰���
    private bool CheckIntersectionRectangle(Collider other)
    {
        Transform myTrans = GetComponent<Transform>();
        Transform otherTrans = other.GetComponent<Transform>();

        if (myTrans is null || otherTrans is null) return false;

        Vector2 mySize = new Vector2(myTrans.localScale.x, myTrans.localScale.y);
        Vector2 otherSize = new Vector2(otherTrans.localScale.x, otherTrans.localScale.y);

        Vector2[] basePoints = GetRectangleCorners(myTrans.position, mySize, -myTrans.eulerAngles.y);
        Vector2[] testPoints = GetRectangleCorners(otherTrans.position, otherSize, -otherTrans.eulerAngles.y);

        basePointsTest = (Vector2[])basePoints.Clone();
        testPointsTest = (Vector2[])testPoints.Clone();
    
        List<Vector2> intersection = GetIntersectionPolygon(basePoints, testPoints);
        intersectPolygon = intersection;
        float intersectArea = CalculatePolygonArea(intersection);
        float myArea = myTrans.localScale.x * myTrans.localScale.y /** 2f*/;

        Debug.Log($"intersectionArea: {intersectArea}, baseArea: {myArea}, percentages: {intersectArea/myArea}");
        if (intersectArea / myArea >= 0.3f) return true;

        return false;
    }



    private Vector2[] GetRectangleCorners(Vector3 center, Vector2 size, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(-size.x * 0.5f, size.y * 0.5f); // left top
        corners[1] = new Vector2(size.x * 0.5f, size.y * 0.5f);   // right top
        corners[2] = new Vector2(size.x * 0.5f, -size.y * 0.5f);  // right bottom
        corners[3] = new Vector2(-size.x * 0.5f, -size.y * 0.5f);   // left bottom

        // ȸ���� ��ǥ ���
        for (int i = 0; i < 4; i++)
        {
            float x = corners[i].x * cos - corners[i].y * sin + center.x;
            float y = corners[i].x * sin + corners[i].y * cos + center.z;
            corners[i] = new Vector2(x, y);
        }

        return corners;
    }

    public int testCounts = 1;

    private List<Vector2> GetIntersectionPolygon(Vector2[] basePoints, Vector2[] testPoints)
    {
        List<Vector2> intersectionPoints = new List<Vector2>(testPoints);
        for (int i = 0; i < testCounts/*basePoints.Length*/; i++)
        {
            Vector2 p1 = basePoints[i];
            Vector2 p2 = basePoints[(i + 1) % basePoints.Length];
            intersectionPoints = ClipPolygon(intersectionPoints, p1, p2);
            //if (outputList.Count < 3) return new List<Vector2>();
        }
        return intersectionPoints;
    }

    private List<Vector2> ClipPolygon(List<Vector2> testPoints, Vector2 edgeStart, Vector2 edgeEnd)
    {
        List<Vector2> haveToCheckPoints = new List<Vector2>();
        Vector2 edgeDir = edgeEnd - edgeStart;

        for (int i = 0; i < testPoints.Count; i++)
        {
            Vector2 curr = testPoints[i];
            Vector2 prev = testPoints[(i - 1 + testPoints.Count) % testPoints.Count];

            // ������ clipping ���ؼ����� ������ ���� ��� ����
            //if (Vector3.Cross(edgeDir, curr - prev).z > 0 /*|| Mathf.Abs(Vector3.Cross(edgeDir, curr-prev).z) < Mathf.Epsilon*/) continue;

            bool currInside = Vector3.Cross(edgeDir, curr - edgeStart).z < 0;
            bool prevInside = Vector3.Cross(edgeDir, prev - edgeStart).z < 0;

            if (currInside && prevInside)
            {
                // ���� �� �߰�
                haveToCheckPoints.Add(curr);
            }
            else if (currInside && !prevInside)
            {
                // ���а� clipping ���ؼ��� �����ϴ��� Ȯ��
                if (LineSegmentIntersection(edgeStart, edgeDir, prev, curr, out Vector2 intersection))
                {
                    // ������ ������ ��� üũ�ؾ��� ���� ������ �߰�
                    haveToCheckPoints.Add(intersection);
                    // ���� �� �߰�
                    haveToCheckPoints.Add(curr);
                }
            }
            else if (!currInside && prevInside)
            {
                // ���а� clipping ���ؼ��� �����ϴ��� Ȯ��
                if (LineSegmentIntersection(edgeStart, edgeDir, prev, curr, out Vector2 intersection))
                {
                    // ������ ������ ��� üũ�ؾ��� ���� ������ �߰�
                    haveToCheckPoints.Add(intersection);
                }
            }
            else 
                continue;

        }
        return haveToCheckPoints;
    }

    /// <summary>
    /// ������ ������ �����ϴ��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="linePoint">�������� �� ��</param>
    /// <param name="dir">������ ���� ����</param>
    /// <param name="segStart">������ ������</param>
    /// <param name="segEnd">������ ����</param>
    /// <param name="intersection">�����ϴ� �� ����</param>
    /// <returns></returns>
    private bool LineSegmentIntersection(Vector2 linePoint, Vector2 dir, Vector2 segStart, Vector2 segEnd, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 segDir = segEnd - segStart;
        Vector2 segLineDir = segStart - linePoint;

        float denominator = dir.x * segDir.y - dir.y * segDir.x;

        // �и� 0�̸� �� ���� �����ϰų� ��ġ��
        if (Mathf.Abs(denominator) < Mathf.Epsilon) return false;

        float t = (segLineDir.x * segDir.y - segLineDir.y * segDir.x) / denominator;
        float u = (segLineDir.x * dir.y - segLineDir.y * dir.x) / denominator;

        // u�� [0, 1] ������ �־�� ���а� ������
        if (u < 0 || u > 1) return false;

        // ������ ���
        intersection = linePoint + t * dir;
        return true;
    }

    // https://ko.wikipedia.org/wiki/%EC%8B%A0%EB%B0%9C%EB%81%88_%EA%B3%B5%EC%8B%9D  �Ź߲� ����
    private float CalculatePolygonArea(List<Vector2> polygon)
    {
        if (polygon.Count < 3) return 0;
        float area = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 p1 = polygon[i];
            Vector2 p2 = polygon[(i + 1) % polygon.Count];
            area += (p1.x * p2.y - p2.x * p1.y);
        }
        return Mathf.Abs(area) * 0.5f;
    }

    private List<Vector2> intersectPolygon = new List<Vector2>();

    private void OnDrawGizmos()
    {
        for(int i = 0; i < intersectPolygon.Count; i++)
        {
            Vector3 start = new Vector3(intersectPolygon[i].x, 0f , intersectPolygon[i].y);
            Vector3 end = new Vector3(intersectPolygon[(i + 1) % intersectPolygon.Count].x, 0f, intersectPolygon[(i + 1) % intersectPolygon.Count].y);

            if(isAbleFusion)
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawLine(start, end);
        }
    }

    private bool CheckIntersectBetweenRectangleCircle(Collider other)
    {
        Transform myTrans = GetComponent<Transform>();
        Transform otherTrans = other.GetComponent<Transform>();

        Vector2 mySize = new Vector2(myTrans.localScale.x, myTrans.localScale.y);
        Vector2[] rect = GetRectangleCorners(myTrans.position, mySize, -myTrans.eulerAngles.y);

        // ���� �߽��� �簢�� �ȿ� ���ԵǾ� �ִ� ��� ������ �׻� 25%�� �ѱ⿡ true ��ȯ
        Vector2 circleCenter = new Vector2(otherTrans.position.x, otherTrans.position.z);
        Vector2 rectCenter = new Vector2(myTrans.position.x, myTrans.position.z);
        if (CheckPointInRect(circleCenter, rect)) return true;

        float intersectArea = CalculateIntersectAreaRectAndCircle(new Vector2(otherTrans.position.x, otherTrans.position.z), other.transform.localScale.x * 0.5f, rect, rectCenter);
        float rectArea = myTrans.localScale.x * myTrans.localScale.y /** 2f*/;
        float circleArea = Mathf.Pow(other.transform.localScale.x * 0.5f, 2) * Mathf.PI;

        if (intersectArea / rectArea >= 0.25f || intersectArea / circleArea >= 0.25f) return true;

        return false;
    }

    private bool CheckPointInRect(Vector2 center, Vector2[] rect)
    {
        List<Vector2> intersectionPoints = new List<Vector2>() {center};
        for (int i = 0; i < testCounts/*basePoints.Length*/; i++)
        {
            Vector2 p1 = rect[i];
            Vector2 p2 = rect[(i + 1) % rect.Length];
            intersectionPoints = ClipPolygon(intersectionPoints, p1, p2);
        }

        return intersectionPoints.Count == 1 ? true : false;
    }

    private float CalculateIntersectAreaRectAndCircle(Vector2 circleCenter, float radius, Vector2[] rect, Vector2 rectCenter)
    {
        // 1. �簢���� �� ���� ���� ���� ã��
        List<Vector2> intersectionPoints = FindCircleRectangleIntersections(circleCenter, radius, rect);

        // 2. ������ ������ ���Ե� �����̱⿡ �簢���� ���� �״�� ��ȯ
        if (intersectionPoints.Count == 0)
            return Mathf.Abs(rect[1].x - rect[0].x) * Mathf.Abs(rect[2].y - rect[1].y);
        

        // 3. ���� ������ �ٰ������� ����
        List<Vector2> clipPolygon = CreateIntersectionPolygon(circleCenter, rectCenter, radius, rect, intersectionPoints);
        intersectPolygon = clipPolygon;

        float polygonArea = 0f, segmentArea = 0f;

        // 4. ������ ������ ���� ���� ���
        // https://mvtrinh.wordpress.com/2018/09/22/circle-and-square-intersections/
        switch (intersectionPoints.Count)
        {
            // ������ ������ 1���� ��� 0 ��ȯ(1���� ���� ���� �簢���� ���ϴ� ���)
            // ���� �߽��� �簢���ȿ� ���ԵǴ� ���� �̹� ���ܸ� �߱⿡ �� ���� ���� �簢���� ��ġ�� ������ �������� �ʴ´�.
            case 1:
                break;

            // ������ ������ 2, 3���� ���
            case 2: 
            case 3:
                polygonArea += CalculatePolygonArea(clipPolygon);
                segmentArea += CalculateCircularSegmentArea(circleCenter, radius, intersectionPoints);
                break;

            // ������ ������ 4���� ���(4���� ���� �簢���� ��κп� ������ �̹� ���� ���ο� ���ԵǾ� �ֱ⿡,
            // �ٰ����� ���� �����ε� 30%�̻��� �ȴ�.
            default:
                polygonArea += CalculatePolygonArea(clipPolygon);
                break;

        }
        return polygonArea + segmentArea;
    }

    private List<Vector2> FindCircleRectangleIntersections(Vector2 circleCenter, float radius, Vector2[] rect)
    {
        List<Vector2> points = new List<Vector2>();

        // �簢���� �� ���� ������ ���������� ����
        for(int i = 0; i < rect.Length; i++)
        {
            Vector2 p1 = rect[i];
            Vector2 p2 = rect[(i + 1)% rect.Length];

            List<Vector2> intersections = CircleLineIntersection(circleCenter, radius, p1, p2);
            points.AddRange(intersections);
        }

        return points;
    }

    private List<Vector2> CircleLineIntersection(Vector2 circleCenter, float radius, Vector2 p1, Vector2 p2)
    {
        List<Vector2> intersections = new List<Vector2>();

        Vector2 d = p2 - p1;
        Vector2 f = p1 - circleCenter;

        float a = Vector2.Dot(d, d);
        float b = 2 *  Vector2.Dot(f, d);
        float c = Vector2.Dot(f, f) - radius * radius;

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
            return intersections;

        discriminant = Mathf.Sqrt(discriminant);

        float t1 = (-b - discriminant) / (2 * a);
        float t2 = (-b + discriminant) / (2 * a);

        if (t1 >= 0 && t1 <= 1)
            intersections.Add(p1 + t1 * d);
        if (t2 >= 0 && t2 <= 1)
            intersections.Add(p1 + t2 * d);

        return intersections;
    }

    private List<Vector2> CreateIntersectionPolygon(Vector2 circleCenter, Vector2 rectCenter, float radius, Vector2[] rect, List<Vector2> intersections)
    {
        List<Vector2> polygon = new List<Vector2>();

        foreach (Vector2 point in intersections)
            polygon.Add(point);

        // �� ���ο� �ִ� �簢���� �� �߰�
        foreach (Vector2 p in rect)
        {
            if ((p - circleCenter).sqrMagnitude <= radius * radius)
                polygon.Add(p);
        }

        basePointsTest = polygon.ToArray();
        // �ٰ����� �簢���� �߽� �������� ����
        polygon.Sort((a, b) =>
        {
        float angleA = Mathf.Atan2(a.y - rectCenter.y, a.x - rectCenter.x) * Mathf.Rad2Deg;
            if (angleA < 0) angleA += 360f;
            float angleB = Mathf.Atan2(b.y - rectCenter.y, b.x - rectCenter.x) * Mathf.Rad2Deg;
            if (angleB < 0) angleB += 360f;
            return angleA.CompareTo(angleB);
        });

        testPointsTest = polygon.ToArray();
        return polygon;
    }   
    
    float CalculateCircularSegmentArea(Vector2 center, float radius, List<Vector2> intersections)
    {
        float arcArea = 0f;

        for(int i = 0; i < intersections.Count-1; i++)
        {
            Vector2 p1 = intersections[i] - center;
            Vector2 p2 = intersections[(i+1) % intersections.Count] - center;

            float angle = Vector2.Angle(p1, p2) * Mathf.Deg2Rad;
            float segmentArea = 0.5f * radius * radius * (angle - Mathf.Sin(angle));

            arcArea += Mathf.Abs(segmentArea);
        }

        return arcArea;
    }
}
