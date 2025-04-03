using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InkMarkType
{
    BASICATTACK,
    DASH,
    INKSKILL,
    INTERACTIVEOBJECT
}

public class InkMarkSetter : Singleton<InkMarkSetter>
{
    public float inkFusionIntersectionAreaThreshold = 0.25f;
    public InkTypeSO inkTypeData; // 0: BA, 1: Dash, 2: Skill, 3: InteractiveObject
    public InkMaskSO inkMask;
    public static int inkId = 1;

    public InkType SetMergedInkType(InkType inkTypeA, InkType inkTypeB)
    {
        if (inkTypeA == InkType.RED && inkTypeB == InkType.GREEN) return InkType.FIRE;
        if (inkTypeA == InkType.GREEN && inkTypeB == InkType.RED) return InkType.FIRE;
        if (inkTypeA == InkType.BLUE && inkTypeB == InkType.GREEN) return InkType.SWAMP;
        if (inkTypeA == InkType.GREEN && inkTypeB == InkType.BLUE) return InkType.SWAMP;
        if (inkTypeA == InkType.BLUE && inkTypeB == InkType.RED) return InkType.MIST;
        if (inkTypeA == InkType.RED && inkTypeB == InkType.BLUE) return InkType.MIST;
        return InkType.FIRE;
    }

    public void SetInkMarkScaleAndDuration(InkMarkType inkMarkType, Transform inkMarkTransform, ref float duration)
    {
        switch (inkMarkType)
        { 
            case InkMarkType.BASICATTACK:
                inkMarkTransform.localScale = new(3, 3, 1);
                duration = 4;
                break;
            case InkMarkType.DASH:
                inkMarkTransform.localScale = Vector3.zero;
                duration = 6;
                break;
            case InkMarkType.INKSKILL:
                inkMarkTransform.localScale = new(3, 3, 1);
                duration = 5;
                break;
            case InkMarkType.INTERACTIVEOBJECT:
                inkMarkTransform.localScale = new(3, 3, 1);
                duration = 7;
                break;
        }
    }
    
    public void AddCollider(InkMarkType inkMarkType, Transform inkMarkTransform)
    {
        switch (inkMarkType)
        {
            case InkMarkType.BASICATTACK:
            case InkMarkType.INKSKILL:
            case InkMarkType.INTERACTIVEOBJECT:
                SphereCollider sphereCollider = inkMarkTransform.AddComponent<SphereCollider>();
                sphereCollider.radius = 0.5f;
                sphereCollider.isTrigger = true;
                break;
            case InkMarkType.DASH:
                BoxCollider boxCollider = inkMarkTransform.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(1f, 1f, 0f);
                boxCollider.isTrigger = true;
                boxCollider.center = new Vector3(0f, 0.05f, 0f);
                break;
        }
    }

    public bool SetInkMarkSprite(InkMarkType inkMarkType, InkType inkType, SpriteRenderer inkMarkSpriteRenderer, SpriteMask spriteMask)
    {
        bool result = false;

        switch (inkMarkType)
        {
            case InkMarkType.BASICATTACK:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 2, inkId++);
                break;
            case InkMarkType.DASH:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 1, inkId++);
                break;
            case InkMarkType.INKSKILL:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 0, inkId++);
                break;
            case InkMarkType.INTERACTIVEOBJECT:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 0, inkId++);
                break;
        }

        return result;
    }

    private bool SetSprite(InkType inkType, SpriteRenderer spriteRenderer, SpriteMask spriteMask, int maskIndex, int inkId)
    {
        switch (inkType)
        {
            case InkType.RED:
                spriteRenderer.sprite = inkTypeData.images[0];
                break;
            case InkType.GREEN:
                spriteRenderer.sprite = inkTypeData.images[1];
                break;
            case InkType.BLUE:
                spriteRenderer.sprite = inkTypeData.images[2];
                break;
            case InkType.FIRE:
                spriteRenderer.sprite = inkTypeData.images[3];
                break;
            case InkType.MIST:
                spriteRenderer.sprite = inkTypeData.images[4];
                break;
            case InkType.SWAMP:
                spriteRenderer.sprite = inkTypeData.images[5];
                break;
        }

        spriteMask.sprite = inkMask.images[maskIndex];

        // Set Unique Sorting Order (inkId based)
        int baseOrder = inkId * 10;
        spriteRenderer.sortingOrder = baseOrder + 1;

        spriteMask.frontSortingOrder = baseOrder + 1;
        spriteMask.backSortingOrder = baseOrder;
        spriteMask.isCustomRangeActive = true;

        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Failed to assign the intended ink mark SpriteImage.");
            return false;
        }

        return true;
    }

    public Sprite SetSprite(InkType inkType)
    {
        if (inkType == InkType.FIRE)  return inkTypeData.images[3];
        if (inkType == InkType.MIST)  return inkTypeData.images[4];
        if (inkType == InkType.SWAMP) return inkTypeData.images[5];
        return null;
    }

    //https://yupdown.tistory.com/31
    public bool CheckIntersectionBetweenCircles(Collider circle1, Collider circle2)
    {
        Transform circle1Trans = circle1.GetComponent<Transform>();
        Transform circle2Trans = circle2.GetComponent<Transform>();

        if (circle1Trans is null || circle2Trans is null) return false;

        float r1 = circle1.transform.localScale.x * 0.5f;
        float r2 = circle2.transform.localScale.x * 0.5f;

        float distance = Vector3.Distance(circle1Trans.position, circle2Trans.position);

        // Central angle of the intersection area
        float thetaR1 = 2 * Mathf.Acos((distance * distance + r1 * r1 - r2 * r2) / (2 * distance * r1));
        float thetaR2 = 2 * Mathf.Acos((distance * distance + r2 * r2 - r1 * r1) / (2 * distance * r2));

        // Triangle area in the intersection region
        float triangleAreaR1 = r1 * r1 * 0.5f * Mathf.Sin(thetaR1);
        float triangleAreaR2 = r2 * r2 * 0.5f * Mathf.Sin(thetaR2);

        // Sector area of the intersection region
        float fanAreaR1 = r1 * r1 * 0.5f * thetaR1;
        float fanAreaR2 = r2 * r2 * 0.5f * thetaR2;

        // Calculate the total intersection area
        float intersectArea = fanAreaR1 - triangleAreaR1 + fanAreaR2 - triangleAreaR2;
        float circleArea = r1 * r1 * (float)Mathf.PI;

        if (intersectArea / circleArea >= inkFusionIntersectionAreaThreshold) return true;

        return false;
    }

    public bool CheckIntersectionBetweenRectangles(Collider rect1, Collider rect2)
    {
        Transform rect1Trans = rect1.GetComponent<Transform>();
        Transform rect2Trans = rect2.GetComponent<Transform>();

        if (rect1Trans is null || rect2Trans is null) return false;

        Vector2 rect1Size = new(rect1Trans.localScale.x, rect1Trans.localScale.y);
        Vector2 rect2Size = new(rect2Trans.localScale.x, rect2Trans.localScale.y);

        Vector2[] basePoints = GetRectangleCorners(rect1Trans.position, rect1Size, -rect1Trans.eulerAngles.y);
        Vector2[] testPoints = GetRectangleCorners(rect2Trans.position, rect2Size, -rect2Trans.eulerAngles.y);

        List<Vector2> intersection = GetIntersectionPolygon(basePoints, testPoints);
        float intersectArea = CalculatePolygonArea(intersection);

        Debug.Log($"Intersection polygon count: {intersection.Count}");
        Debug.Log($"Intersected Area: {intersectArea}");
        float rect1Area = rect1Trans.localScale.x * rect1Trans.localScale.y;
        float rect2Area = rect2Trans.localScale.x * rect2Trans.localScale.y;

        Debug.Log($"Intersect Area: {intersectArea}");
        Debug.Log($"rect1Area: {rect1Area}, rect2Area: {rect2Area}");
        Debug.Log($"Ratio1: {intersectArea / rect1Area}, Ratio2: {intersectArea / rect2Area}");

        if (intersectArea / rect1Area >= inkFusionIntersectionAreaThreshold
            || intersectArea/ rect2Area >= inkFusionIntersectionAreaThreshold) return true;
        
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

        // Rotate coordinate points
        for (int i = 0; i < 4; i++)
        {
            float x = corners[i].x * cos - corners[i].y * sin + center.x;
            float y = corners[i].x * sin + corners[i].y * cos + center.z;
            corners[i] = new Vector2(x, y);
        }

        return corners;
    }

    private List<Vector2> GetIntersectionPolygon(Vector2[] basePoints, Vector2[] testPoints)
    {
        List<Vector2> intersectionPoints = new List<Vector2>(testPoints);
        for (int i = 0; i < basePoints.Length; i++)
        {
            Vector2 p1 = basePoints[i];
            Vector2 p2 = basePoints[(i + 1) % basePoints.Length];
            intersectionPoints = ClipPolygon(intersectionPoints, p1, p2);
        }
        return intersectionPoints;
    }

    // https://www.sunshine2k.de/coding/java/SutherlandHodgman/SutherlandHodgman.html
    // https://www.youtube.com/watch?v=Euuw72Ymu0M
    private List<Vector2> ClipPolygon(List<Vector2> testPoints, Vector2 edgeStart, Vector2 edgeEnd)
    {
        List<Vector2> haveToCheckPoints = new List<Vector2>();
        Vector2 edgeDir = edgeEnd - edgeStart;

        for (int i = 0; i < testPoints.Count; i++)
        {
            Vector2 curr = testPoints[i];
            Vector2 prev = testPoints[(i - 1 + testPoints.Count) % testPoints.Count];

            bool currInside = Vector3.Cross(edgeDir, curr - edgeStart).z < 0;
            bool prevInside = Vector3.Cross(edgeDir, prev - edgeStart).z < 0;

            if (currInside && prevInside)
            {
                // Add current point
                haveToCheckPoints.Add(curr);
            }
            else if (currInside && !prevInside)
            {
                // Check if it intersects with the clipping edge
                if (LineSegmentIntersection(edgeStart, edgeDir, prev, curr, out Vector2 intersection))
                {
                    // Add intersection point since it must be checked
                    haveToCheckPoints.Add(intersection);
                    // Add current point
                    haveToCheckPoints.Add(curr);
                }
            }
            else if (!currInside && prevInside)
            {
                // Check if it intersects with the clipping edge
                if (LineSegmentIntersection(edgeStart, edgeDir, prev, curr, out Vector2 intersection))
                {
                    // Add intersection point since it must be checked
                    haveToCheckPoints.Add(intersection);
                }
            }
            else
                continue;
        }
        return haveToCheckPoints;
    }

    /// <summary>
    /// Checks whether a line and a segment intersect
    /// http://www.gisdeveloper.co.kr/?p=89
    /// </summary>
    /// <param name="linePoint">A point on the infinite line</param>
    /// <param name="dir">Direction vector of the infinite line</param>
    /// <param name="segStart">Start point of the segment</param>
    /// <param name="segEnd">End point of the segment</param>
    /// <param name="intersection">Intersection point if they intersect</param>
    /// <returns>Returns true if an intersection exists, false otherwise</returns>
    private bool LineSegmentIntersection(Vector2 linePoint, Vector2 dir, Vector2 segStart, Vector2 segEnd, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 segDir = segEnd - segStart;
        Vector2 segLineDir = segStart - linePoint;

        float denominator = dir.x * segDir.y - dir.y * segDir.x;

        // If denominator is 0, the lines are parallel or coincident
        if (Mathf.Abs(denominator) < Mathf.Epsilon) return false;

        float t = (segLineDir.x * segDir.y - segLineDir.y * segDir.x) / denominator;
        float u = (segLineDir.x * dir.y - segLineDir.y * dir.x) / denominator;

        // If u is within [0,1], the segment intersects with the line
        if (u < 0 || u > 1) return false;

        // Compute intersection point
        intersection = linePoint + t * dir;
        return true;
    }

    // https://ko.wikipedia.org/wiki/%EC%8B%A0%EB%B0%9C%EB%81%88_%EA%B3%B5%EC%8B%9D  Shoelace formula
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

    public bool CheckIntersectionBetweenRectangleCircle(Collider rect, Collider circle)
    {
        Transform rectTrans = rect.GetComponent<Transform>();
        Transform circleTrans = circle.GetComponent<Transform>();

        if (rectTrans is null || circleTrans is null) return false;

        Vector2 rectSize = new Vector2(rectTrans.localScale.x, rectTrans.localScale.y);
        Vector2[] rectPos = GetRectangleCorners(rectTrans.position, rectSize, -rectTrans.eulerAngles.y);

        // If a random point is inside a square centered on the origin, 
        // there is always a 25% probability of returning true.
        Vector2 circleCenter = new Vector2(circleTrans.position.x, circleTrans.position.z);
        float circleRadius = circleTrans.transform.localScale.x * 0.5f;
        if (CheckPointInRect(circleCenter, rectPos)) return true;

        float intersectArea = CalculateIntersectAreaRectAndCircle(circleCenter, circleRadius, rectPos);
        float rectArea = rectTrans.localScale.x * rectTrans.localScale.y /** 2f*/;
        float circleArea = circleRadius * circleRadius * Mathf.PI;

        if (intersectArea / rectArea >= inkFusionIntersectionAreaThreshold 
            || intersectArea / circleArea >= inkFusionIntersectionAreaThreshold) return true;

        return false;
    }

    private bool CheckPointInRect(Vector2 center, Vector2[] rect)
    {
        List<Vector2> intersectionPoints = new List<Vector2>() { center };
        for (int i = 0; i < rect.Length; i++)
        {
            Vector2 p1 = rect[i];
            Vector2 p2 = rect[(i + 1) % rect.Length];
            intersectionPoints = ClipPolygon(intersectionPoints, p1, p2);
        }

        return intersectionPoints.Count == 1 ? true : false;
    }

    private float CalculateIntersectAreaRectAndCircle(Vector2 circleCenter, float radius, Vector2[] rect)
    {
        // 1. Find the intersection points between the rectangle and the circle
        List<Vector2> intersectionPoints = FindCircleRectangleIntersections(circleCenter, radius, rect);

        // 2. If there are no intersections, the rectangle is fully inside the circle, return its area
        if (intersectionPoints.Count == 0)
            return Mathf.Abs(rect[1].x - rect[0].x) * Mathf.Abs(rect[2].y - rect[1].y);

        // 3. Construct the clipped polygon formed by the intersection
        List<Vector2> clipPolygon = CreateIntersectionPolygon(circleCenter, radius, rect, intersectionPoints);

        float polygonArea = 0f, segmentArea = 0f;

        // 4. Calculate the intersection area
        // Reference: https://mvtrinh.wordpress.com/2018/09/22/circle-and-square-intersections/
        switch (intersectionPoints.Count)
        {
            // If there is only one intersection point, return 0 (the rectangle is mostly outside the circle)
            // A single intersection means the circle does not fully enclose any part of the rectangle.
            case 1:
                break;

            // If there are 2 or 3 intersection points
            case 2:
            case 3:
                polygonArea += CalculatePolygonArea(clipPolygon);
                segmentArea += CalculateCircularSegmentArea(circleCenter, radius, intersectionPoints);
                break;

            // If there are 4 intersection points (the rectangle is partially inside the circle),
            // the overlapping area is likely more than 30%.
            default:
                polygonArea += CalculatePolygonArea(clipPolygon);
                break;
        }
        return polygonArea + segmentArea;
    }

    private List<Vector2> FindCircleRectangleIntersections(Vector2 circleCenter, float radius, Vector2[] rect)
    {
        List<Vector2> points = new List<Vector2>();

        // Calculate the intersection area between the rectangle and the circle
        for (int i = 0; i < rect.Length; i++)
        {
            Vector2 p1 = rect[i];
            Vector2 p2 = rect[(i + 1) % rect.Length];

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
        float b = 2 * Vector2.Dot(f, d);
        float c = Vector2.Dot(f, f) - radius * radius;

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0) return intersections;

        discriminant = Mathf.Sqrt(discriminant);

        float t1 = (-b - discriminant) / (2 * a);
        float t2 = (-b + discriminant) / (2 * a);

        if (t1 >= 0 && t1 <= 1)
            intersections.Add(p1 + t1 * d);
        if (t2 >= 0 && t2 <= 1)
            intersections.Add(p1 + t2 * d);

        return intersections;
    }

    private List<Vector2> CreateIntersectionPolygon(Vector2 center, float radius, Vector2[] rect, List<Vector2> intersections)
    {
        List<Vector2> polygon = new List<Vector2>();

        foreach (Vector2 point in intersections)
            polygon.Add(point);

        // Add the rectangle that exists in the list
        foreach (Vector2 p in rect)
        {
            if ((p - center).sqrMagnitude <= radius * radius) polygon.Add(p);
        }

        // Calculate the center position of the clipped area
        polygon.Sort((a, b) => Mathf.Atan2(a.y - center.y, a.x - center.x)
        .CompareTo(Mathf.Atan2(b.y - center.y, b.x - center.x)));

        return polygon;
    }

    private float CalculateCircularSegmentArea(Vector2 center, float radius, List<Vector2> intersections)
    {
        float arcArea = 0f;

        for (int i = 0; i < intersections.Count - 1; i++)
        {
            Vector2 p1 = intersections[i] - center;
            Vector2 p2 = intersections[(i + 1) % intersections.Count] - center;

            float angle = Vector2.Angle(p1, p2) * Mathf.Deg2Rad;
            float segmentArea = 0.5f * radius * radius * (angle - Mathf.Sin(angle));

            arcArea += Mathf.Abs(segmentArea);
        }

        return arcArea;
    }
}