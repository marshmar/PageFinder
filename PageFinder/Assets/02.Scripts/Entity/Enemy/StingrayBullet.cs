using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StingrayBullet : MonoBehaviour
{
    Rigidbody rb;
    int speed = 10;

    bool canMove = false; 
    int parentNum = 0; // 이 총알을 생성해낸 Stingray 객체의 번호 

    Vector3 targetDir = Vector3.zero;

    Player playerScr;
    Stingray stingray;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerScr = GameObject.FindWithTag("PLAYER").GetComponent<Player>();
        stingray = GameObject.Find("Enemies").transform.GetChild(parentNum).GetComponent<Stingray>();
        canMove = false;
    }

    private void Update()
    {
        if(canMove)
            rb.MovePosition(rb.position + targetDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER"))
        {
            playerScr.HP -= stingray.ATK;
            //Debug.Log("PLAYER HP: " + playerScr.HP);
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            stingray.BulletIndex--;
        }
        else if (coll.CompareTag("MAP"))
        {
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            stingray.BulletIndex--;
        }
    }

    public bool CanMove
    {
        get
        {
            return canMove;
        }
        set
        {
            canMove = value;
        }
    }

    public int ParentNum
    {
        get
        {
            return parentNum;
        }
        set
        {
            parentNum = value;
        }
    }

    public Vector3 TargetDir
    {
        get
        {
            return targetDir;
        }
        set
        {
            targetDir = value;
        }
    }
}
