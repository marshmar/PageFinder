using UnityEngine;

public class InkMarkFusionTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float inkFusionCircleThreshold = 0.3f;

    private void OnTriggerStay(Collider other)
    {

        if (CheckIntersectCircle(other))
            Debug.Log("�ռ� ����");
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

        Debug.Log($"intersectArea: {intersectArea}, CircleArea: {circleArea}");

        if (intersectArea / circleArea >= inkFusionCircleThreshold) return true;

        return false;
    }
}
