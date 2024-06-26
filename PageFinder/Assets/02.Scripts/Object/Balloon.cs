using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public GameObject Particle_Prefab;

    GameObject particle;

    bool didColl = false;

    Palette palette;
    Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        palette = GameObject.FindWithTag("PLAYER").GetComponent<Palette>();

        StartCoroutine(MoveUpDown());
    }

    IEnumerator MoveUpDown()
    {
        Vector3 moveDir = Vector3.up;
        float speed = 1 + Random.Range(0, 10) / 10.0f; // 1.0 ~ 1.9

        while (!didColl)
        {
            if (transform.position.y <= 1.5f)
                moveDir = Vector3.up;    
            else if(transform.position.y > 6)
                moveDir = Vector3.down;

            transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

            yield return null;
        }
    }

    public void ChangeColor()
    {
        if (material.color == Color.red) // 목표한 색깔이 이미 된 경우 동작하지 않도록 함
            return;

        material.color = palette.ReturnCurrentColor(); // 풍선 색깔 현재 플레이어 팔레트 색깔로 변경

        if (material.color == Color.red) // 목표한 색깔인 빨간색으로 변경시에만 MoveUpDown 코루틴이 종료되도록 함 
        {
            particle = Instantiate(Particle_Prefab, transform.position, Quaternion.identity);
            Destroy(particle, 2.0f);
            didColl = true;
        }

    }
}
