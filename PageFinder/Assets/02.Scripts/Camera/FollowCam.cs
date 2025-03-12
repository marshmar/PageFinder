using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;

    [Range(1.0f, 20.0f)]
    public float distance = 1.0f; // ���� ������κ��� ������ �Ÿ�
    public float height = 20.0f; // Y������ �̵��� ����
    public float damping = 10.0f; // ���� �ӵ�

    private Vector3 velocity = Vector3.zero; // SmoothDamp���� ����� ����
    private Transform camTr;

    void Start()
    {
        camTr = GetComponent<Transform>();
        DontDestroyOnLoad(this.gameObject);
    }

    void LateUpdate()
    {
        if (targetTr == null) return; // �÷��̾ �׾��� ���

        Vector3 pos = targetTr.position + new Vector3(0, height, -distance);
        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);
        HideObject();
    }

    /// <summary>
    /// �÷��̾�� ī�޶� ������ �ִ� ������Ʈ�� ������ �Լ�
    /// </summary>
    void HideObject()
    {
        Vector3 direction = (targetTr.position - transform.position).normalized;
        // �÷��̾�� ī�޶� ������ ��� ȯ�� ��ֹ��� ã�´�.
        RaycastHit[] hits = Physics.RaycastAll(
            transform.position, 
            direction, 
            Mathf.Infinity, 
            1 << LayerMask.NameToLayer("EnvironmentObject")
        );

        // ã�� ��ֹ����� ����ȭ�Ѵ�.
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