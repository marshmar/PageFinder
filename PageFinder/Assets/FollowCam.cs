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
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = targetTr.position + new Vector3(0, height, -distance);

        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);

    }
}
