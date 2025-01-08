using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttack : MonoBehaviour
{
    [Header("Proectile")]
    [SerializeField]
    private GameObject Projectile_Prefab;
    [SerializeField]
    private Transform projectilePos;
    [SerializeField]
    private int speed;

    //List 로 변경하여 개수 능동적으로 변경할 수 있게 해보기
    GameObject[] projectile = new GameObject[6];

    private void Start()
    {
        // 투사체 관련
        for (int i = 0; i < projectile.Length; i++)
        {
            projectile[i] = Instantiate(Projectile_Prefab, new Vector3(gameObject.transform.position.x, -10, gameObject.transform.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform);
            projectile[i].GetComponent<Projectile>().Init(gameObject, gameObject.name + " - Projectile" + i, speed, projectilePos);
        }
    }

    private void FireProjectileObject()
    {
        int projectileIndex = FindBulletThatCanBeUsed();
        if (projectileIndex == -1) // 사용할 수 있는 총알이 없을 경우 
        {
            Debug.Log("사용할 총알 부족");
            return;
        }
        projectile[projectileIndex].SetActive(true);
        projectile[projectileIndex].GetComponent<Projectile>().Init(projectilePos.position - transform.position);
    }

    /// <summary>
    /// 사용할 수 있는 투사체를 찾는다.
    /// </summary>
    /// <returns>-1 : 사용할 수 있는 투사체 없음 / 0~Bullet.Length-1 : 사용할 수 있는 투사체 인덱스</returns>
    int FindBulletThatCanBeUsed()
    {
        for (int i = 0; i < projectile.Length; i++)
        {
            if (projectile[i].activeSelf) // 사용중인 총알 
                continue;
            return i;
        }
        return -1;
    }
}
