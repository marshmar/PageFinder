using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    private Transform camTr;

    // 따라갈 대상으로부터 떨어질 거리
    [Range(1.0f, 20.0f)]
    public float distance = 1.0f;

    // Y축으로 이동할 높이
    public float height = 20.0f;

    // 반응 속도
    public float damping = 10.0f;

    // SmoothDamp에서 사용할 변수
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        camTr = GetComponent<Transform>();
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
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
