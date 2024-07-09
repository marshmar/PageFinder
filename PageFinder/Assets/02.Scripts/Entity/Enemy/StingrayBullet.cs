using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StingrayBullet : MonoBehaviour
{
    Rigidbody rb;
    int speed = 10;

    string parentName; // 이 총알을 생성해낸 Stingray 객체의 번호 

    Vector3 targetDir = Vector3.zero;

    Player playerScr;
    Stingray stingray;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerScr = GameObject.FindWithTag("PLAYER").GetComponent<Player>();
    }

    private void Update()
    {
        rb.MovePosition(rb.position + targetDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER"))
        {
            playerScr.HP -= stingray.ATK;
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
        }
        else if (coll.CompareTag("MAP") || coll.CompareTag("OBJECT"))
        {
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 투사체에 기본 값에 대해 초기화한다.
    /// </summary>
    public void Init()
    {
        stingray = GameObject.Find(parentName).GetComponent<Stingray>();
        transform.position = new Vector3(stingray.transform.position.x, 2, stingray.transform.position.z);
        Vector3 bulletDir = (playerScr.transform.position - stingray.transform.position).normalized;
        bulletDir.y = 0;
        targetDir = bulletDir;
    }

    public string ParentName
    {
        get
        {
            return parentName;
        }
        set
        {
            parentName = value;
        }
    }
}
