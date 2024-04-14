using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // 플레이어 애니메이터
    private Animator anim;
    // 이동 방향 벡터
    [SerializeField]
    private Vector3 moveDir;

    // 이동속도 
    public float moveSpeed = 10.0f;
    // 공격 사거리 표시를 위한 오브젝트
    public GameObject rangeObject;
    // 주위의 적 객체를 저장할 콜라이더 배열
    Collider[] enemies;
    // 공격할 적 객체
    Collider attackEnemy;
    // 공격 사거리
    private float attackDist = 2.6f;
    bool isAttack;
    Transform tr;
    Rigidbody rigid;
    RaycastHit rayHit;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
        rangeObject.SetActive(false);
        attackEnemy = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveDir != Vector3.zero)
        {
            if (!isAttack)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
            }
        }
    }
   
    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }

    public IEnumerator OnAttack(InputValue value)
    {
        rangeObject.SetActive(true);
        anim.SetTrigger("Attack");
        FindMinDistanceEnemy(tr.position);
        if (attackEnemy != null)
        {
            Vector3 enemyDir = attackEnemy.gameObject.transform.position - tr.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));
        }
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        rangeObject.SetActive(false);
        isAttack = false;
    }

    public void FindMinDistanceEnemy(Vector3 position)
    {
        attackEnemy = null;
        float minDist = attackDist;
        enemies = Physics.OverlapSphere(position, attackDist, 1 << 6);
        for(int i = 0; i < enemies.Length; i++)
        {
            float dist = Vector3.Distance(position, enemies[i].gameObject.transform.position);
            if(minDist >= dist)
            {
                Debug.Log("enemy Found");
                attackEnemy = enemies[i];
                minDist = dist;
                isAttack = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isAttack)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tr.position, attackDist);
        }
       
    }
}
