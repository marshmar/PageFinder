using UnityEngine;

public class InkMarkFusionTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float inkFusionCircleThreshold = 0.3f;

    private void OnTriggerStay(Collider other)
    {

        if (CheckIntersectCircle(other))
            Debug.Log("합성 가능");
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

        Debug.Log($"intersectArea: {intersectArea}, CircleArea: {circleArea}");

        if (intersectArea / circleArea >= inkFusionCircleThreshold) return true;

        return false;
    }
}
