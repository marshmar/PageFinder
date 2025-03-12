using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;

    [Range(1.0f, 20.0f)]
    public float distance = 1.0f; // 따라갈 대상으로부터 떨어질 거리
    public float height = 20.0f; // Y축으로 이동할 높이
    public float damping = 10.0f; // 반응 속도

    private Vector3 velocity = Vector3.zero; // SmoothDamp에서 사용할 변수
    private Transform camTr;

    void Start()
    {
        camTr = GetComponent<Transform>();
        DontDestroyOnLoad(this.gameObject);
    }

    void LateUpdate()
    {
        if (targetTr == null) return; // 플레이어가 죽었을 경우

        Vector3 pos = targetTr.position + new Vector3(0, height, -distance);
        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);
        HideObject();
    }

    /// <summary>
    /// 플레이어와 카메라 사이의 있는 오브젝트를 가리는 함수
    /// </summary>
    void HideObject()
    {
        Vector3 direction = (targetTr.position - transform.position).normalized;
        // 플레이어와 카메라 사이의 모든 환경 장애물을 찾는다.
        RaycastHit[] hits = Physics.RaycastAll(
            transform.position, 
            direction, 
            Mathf.Infinity, 
            1 << LayerMask.NameToLayer("EnvironmentObject")
        );

        // 찾은 장애물들을 투명화한다.
        for(int i = 0; i <hits.Length; i++)
        {
            TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();
            for(int j = 0; j < obj.Length; j++)
            {
                obj[j].BecomeTransParent();
            }
        }
    }
}