using UnityEngine;

public class Portal : MonoBehaviour
{
    private Vector3 targetPosition;

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 플레이어가 포탈에 접근했을 때만 이동
        {
            Debug.Log($"포탈을 통해 {targetPosition}으로 이동합니다.");
            other.transform.position = targetPosition;
        }
    }
}