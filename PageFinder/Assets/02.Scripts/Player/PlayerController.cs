using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;

public class PlayerController: MonoBehaviour, IPlayerController
{
    #region Move
    private Vector3 moveDir;
    public float moveSpeed = 10.0f;
    #endregion

    #region Attack
    public GameObject rangeObject;      // 공격 사거리 표시
    public GameObject targetObject;     // 타겟팅 표시

    [SerializeField]
    Vector3 attackDir;
    // 주위의 적 객체를 저장할 콜라이더 배열
    Collider[] enemies;
    // 공격할 적 객체
    Collider attackEnemy;
    float attackDist = 2.6f;
    bool isAttack;
    bool targeting;
    #endregion

    #region Component
    private Animator anim;
    Transform tr;
    Rigidbody rigid;
    #endregion

    UtilsManager utilsManager;
    RaycastHit rayHit;

    // Start is called before the first frame update
    void Start()
    {
        Hasing();

        targeting = false;
        rangeObject.SetActive(false);
        attackEnemy = null;
        utilsManager = UtilsManager.Instance;
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

        if (targeting)
        {
            StartCoroutine(SetTarget());
        }
    }
   
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }

    public void ButtonAttack(InputAction.CallbackContext context)
    {

        rangeObject.SetActive(true);
        if (context.performed)
        {
            Debug.Log("button Attack");
            anim.SetTrigger("Attack");

            // 가장 가까운 거리의 적 찾기
            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackDist, 1 << 6);
            if (attackEnemy == null) return;

            // 가까운 거리의 적 방향으로 캐릭터 회전
            Vector3 enemyDir = attackEnemy.gameObject.transform.position - tr.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));

        }
        if (context.canceled)
        {
            rangeObject.SetActive(false);
        }
    }
    public void JoystickAttack(InputAction.CallbackContext context)
    {
        rangeObject.SetActive(true);

        Vector2 inputVec = context.ReadValue<Vector2>();
        if(context.action.phase == InputActionPhase.Started)
        {
            targetObject.transform.position = tr.position;
            targeting = true;
            attackDir = new Vector3(inputVec.x, 0, inputVec.y);
        }
        if (context.action.phase == InputActionPhase.Canceled)
        {
            targeting = false;
        }
    }

    public IEnumerator SetTarget()
    {
        targetObject.SetActive(true);
        if(Vector3.Distance(tr.position, targetObject.transform.position) >= attackDist)
        {
            targetObject.transform.position = targetObject.transform.position;
        }
        else
        {
            targetObject.transform.rotation = Quaternion.LookRotation(attackDir);
            targetObject.transform.Translate(Vector3.forward * 0.2f);
        }
        
        yield return new WaitUntil(() => targeting == false);

        targetObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (isAttack)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tr.position, attackDist);
        }
       
    }

    public void Hasing()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
    }
}
