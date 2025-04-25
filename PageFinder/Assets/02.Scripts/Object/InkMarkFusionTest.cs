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
            Debug.Log("원, 원 합성 모드");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            testNum = 1;
            Debug.Log("사각형, 사각형 합성 모드");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            testNum = 2;
            Debug.Log("사각형, 원 합성 모드");
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
            Debug.Log("두 원이 만나지 않음");
            return false;  // 두 원이 어떤 부분도 교차하지 않는 경우
        }

        // 겹치는 부분의 중심각
        float thetaR1 = 2 * Mathf.Acos((distance * distance + r1 * r1 - r2 * r2) / (2 * distance * r1));
        float thetaR2 = 2 * Mathf.Acos((distance * distance + r2 * r2 - r1 * r1) / (2 * distance * r2));

        // 겹치는 부분의 삼각형 넓이
        float triangleAreaR1 = r1 * r1 * 0.5f * Mathf.Sin(thetaR1);
        float triangleAreaR2 = r2 * r2 * 0.5f * Mathf.Sin(thetaR2);

        // 겹치는 부분의 부채꼴의 넓이
        float fanAreaR1 = r1 * r1 * 0.5f * thetaR1;
        float fanAreaR2 = r2 * r2 * 0.5f * thetaR2;

        // 최종적으로 겹치는 부분의 넓이
        float intersectArea = fanAreaR1 - triangleAreaR1 + fanAreaR2 - triangleAreaR2;
        float circleArea = r1 * r1 * (float)Mathf.PI;

        //Debug.Log($"intersectArea: {intersectArea}, CircleArea: {circleArea}");

        if (intersectArea / circleArea >= inkFusionCircleThreshold) return true;

        return false;
    }

    Vector2[] basePointsTest;
    Vector2[] testPointsTest;

    // https://en.wikipedia.org/wiki/Sutherland%E2%80%93Hodgman_algorithm  서덜랜드-허지맨 Clipping 알고리즘
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

        // 회전된 좌표 계산
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

            // 선분이 clipping 기준선보다 좌측에 있을 경우 제외
            //if (Vector3.Cross(edgeDir, curr - prev).z > 0 /*|| Mathf.Abs(Vector3.Cross(edgeDir, curr-prev).z) < Mathf.Epsilon*/) continue;

            bool currInside = Vector3.Cross(edgeDir, curr - edgeStart).z < 0;
            bool prevInside = Vector3.Cross(edgeDir, prev - edgeStart).z < 0;

            if (currInside && prevInside)
            {
                // 내부 점 추가
                haveToCheckPoints.Add(curr);
            }
            else if (currInside && !prevInside)
            {
                // 선분과 clipping 기준선이 교차하는지 확인
                if (LineSegmentIntersection(edgeStart, edgeDir, prev, curr, out Vector2 intersection))
                {
                    // 선분이 교차할 경우 체크해야할 점에 교차점 추가
                    haveToCheckPoints.Add(intersection);
                    // 내부 점 추가
                    haveToCheckPoints.Add(curr);
                }
            }
            else if (!currInside && prevInside)
            {
                // 선분과 clipping 기준선이 교차하는지 확인
                if (LineSegmentIntersection(edgeStart, edgeDir, prev, curr, out Vector2 intersection))
                {
                    // 선분이 교차할 경우 체크해야할 점에 교차점 추가
                    haveToCheckPoints.Add(intersection);
                }
            }
            else 
                continue;

        }
        return haveToCheckPoints;
    }

    /// <summary>
    /// 직선과 선분이 교차하는지 확인하는 함수
    /// </summary>
    /// <param name="linePoint">직선위의 한 점</param>
    /// <param name="dir">직선의 방향 벡터</param>
    /// <param name="segStart">선분의 시작점</param>
    /// <param name="segEnd">선분의 끝점</param>
    /// <param name="intersection">교차하는 점 저장</param>
    /// <returns></returns>
    private bool LineSegmentIntersection(Vector2 linePoint, Vector2 dir, Vector2 segStart, Vector2 segEnd, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 segDir = segEnd - segStart;
        Vector2 segLineDir = segStart - linePoint;

        float denominator = dir.x * segDir.y - dir.y * segDir.x;

        // 분모가 0이면 두 선이 평행하거나 일치함
        if (Mathf.Abs(denominator) < Mathf.Epsilon) return false;

        float t = (segLineDir.x * segDir.y - segLineDir.y * segDir.x) / denominator;
        float u = (segLineDir.x * dir.y - segLineDir.y * dir.x) / denominator;

        // u가 [0, 1] 범위에 있어야 선분과 교차함
        if (u < 0 || u > 1) return false;

        // 교차점 계산
        intersection = linePoint + t * dir;
        return true;
    }

    // https://ko.wikipedia.org/wiki/%EC%8B%A0%EB%B0%9C%EB%81%88_%EA%B3%B5%EC%8B%9D  신발끈 공식
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

        // 원의 중심이 사각형 안에 포함되어 있는 경우 면적은 항상 25%를 넘기에 true 반환
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
        // 1. 사각형의 네 변과 원의 교점 찾기
        List<Vector2> intersectionPoints = FindCircleRectangleIntersections(circleCenter, radius, rect);

        // 2. 교점이 없으면 포함된 상태이기에 사각형의 면적 그대로 반환
        if (intersectionPoints.Count == 0)
            return Mathf.Abs(rect[1].x - rect[0].x) * Mathf.Abs(rect[2].y - rect[1].y);
        

        // 3. 교차 영역을 다각형으로 구성
        List<Vector2> clipPolygon = CreateIntersectionPolygon(circleCenter, rectCenter, radius, rect, intersectionPoints);
        intersectPolygon = clipPolygon;

        float polygonArea = 0f, segmentArea = 0f;

        // 4. 교점에 개수에 따라 면적 계산
        // https://mvtrinh.wordpress.com/2018/09/22/circle-and-square-intersections/
        switch (intersectionPoints.Count)
        {
            // 교점의 개수가 1개일 경우 0 반환(1개일 경우는 원과 사각형이 접하는 경우)
            // 원의 중심이 사각형안에 포함되는 경우는 이미 제외를 했기에 이 경우는 원과 사각형이 겹치는 영역이 존재하지 않는다.
            case 1:
                break;

            // 교점의 개수가 2, 3개일 경우
            case 2: 
            case 3:
                polygonArea += CalculatePolygonArea(clipPolygon);
                segmentArea += CalculateCircularSegmentArea(circleCenter, radius, intersectionPoints);
                break;

            // 교점의 개수가 4개일 경우(4개일 경우는 사각형의 대부분에 영역이 이미 원의 내부에 포함되어 있기에,
            // 다각형의 영역 만으로도 30%이상이 된다.
            default:
                polygonArea += CalculatePolygonArea(clipPolygon);
                break;

        }
        return polygonArea + segmentArea;
    }

    private List<Vector2> FindCircleRectangleIntersections(Vector2 circleCenter, float radius, Vector2[] rect)
    {
        List<Vector2> points = new List<Vector2>();

        // 사각형의 네 변을 직선의 방정식으로 정의
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

        // 원 내부에 있는 사각형의 점 추가
        foreach (Vector2 p in rect)
        {
            if ((p - circleCenter).sqrMagnitude <= radius * radius)
                polygon.Add(p);
        }

        basePointsTest = polygon.ToArray();
        // 다각형을 사각형의 중심 기준으로 정렬
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
