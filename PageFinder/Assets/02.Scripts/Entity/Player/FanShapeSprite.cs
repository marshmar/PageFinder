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
    private float angle = 90f;
    private float radius = 1f;
    private bool created;

    public float Angle { get => angle; set => angle = value; }
    public float Radius { get => radius; set => radius = value; }

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    public void CreateFanShape()
    {
        if (created) return;
        created = true;
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();


        // 삼각형을 그릴 때에는 정점이 3개가 필요
        // 중심점 + segments수 만큼 계산하고 다시 중심점으로 돌아가기에 총 원소는 segments + 2개임.
        Vector3[] vertices = new Vector3[segments + 2]; // 중심 + 끝점들
        // segments수만큼 삼각형을 그리고, 각 삼각형에는 정점이 3개가 있으므로 segments * 3개의 정점 필요
        int[] triangles = new int[segments * 3];        // 삼각형들
        Vector2[] uv = new Vector2[vertices.Length];    // uv좌표

        // 중앙점 설정
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);


        // segments 사이의 각도 계산
        float angleStep = Angle / segments;
        // 전체각도의 반만큼 왼쪽으로 이동하여 부채꼴이 중앙에서 시작하도록 함.
        float currentAngle = -Angle / 2;

        // 0은 중심점이므로 1부터 시작해 각 끝점을 계산
        for (int i = 1; i <= segments + 1; i++)
        {
            // 현재 각도를 라디안으로 변환
            float radian = Mathf.Deg2Rad * currentAngle;
            vertices[i] = new Vector3(Mathf.Cos(radian) * Radius, Mathf.Sin(radian) * Radius, 0f);

            // uv좌표 설정
            uv[i] = new Vector2(0.5f + Mathf.Cos(radian) * 0.5f, 0.5f + Mathf.Sin(radian) * 0.5f);

            currentAngle += angleStep;
        }

        for(int i = 0; i < segments; i++)
        {
            // 항상 중심점을 사용
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
