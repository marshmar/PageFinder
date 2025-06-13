using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class FanShapeSprite : MonoBehaviour
{
    [SerializeField]
    private Material material;
    private int segments = 20;
    private bool created;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    public void CreateFanShape(float angle, float raidus)
    {
        if (created) return;
        created = true;
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();


        // �ﰢ���� �׸� ������ ������ 3���� �ʿ�
        // �߽��� + segments�� ��ŭ ����ϰ� �ٽ� �߽������� ���ư��⿡ �� ���Ҵ� segments + 2����.
        Vector3[] vertices = new Vector3[segments + 2]; // �߽� + ������
        // segments����ŭ �ﰢ���� �׸���, �� �ﰢ������ ������ 3���� �����Ƿ� segments * 3���� ���� �ʿ�
        int[] triangles = new int[segments * 3];        // �ﰢ����
        Vector2[] uv = new Vector2[vertices.Length];    // uv��ǥ

        // �߾��� ����
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);


        // segments ������ ���� ���
        float angleStep = angle / segments;
        // ��ü������ �ݸ�ŭ �������� �̵��Ͽ� ��ä���� �߾ӿ��� �����ϵ��� ��.
        float currentAngle = -angle / 2;

        // 0�� �߽����̹Ƿ� 1���� ������ �� ������ ���
        for (int i = 1; i <= segments + 1; i++)
        {
            // ���� ������ �������� ��ȯ
            float radian = Mathf.Deg2Rad * currentAngle;
            vertices[i] = new Vector3(Mathf.Cos(radian) * raidus, Mathf.Sin(radian) * raidus, 0f);

            // uv��ǥ ����
            uv[i] = new Vector2(0.5f + Mathf.Cos(radian) * 0.5f, 0.5f + Mathf.Sin(radian) * 0.5f);

            currentAngle += angleStep;
        }

        for(int i = 0; i < segments; i++)
        {
            // �׻� �߽����� ���
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
        mf.mesh = mesh;
    }

}
