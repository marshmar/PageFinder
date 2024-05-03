using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;

/*
 * 수정해야할 사항 
 * 1. 클래스 분할 고려
 *
 */
public class PlayerController: MonoBehaviour, IPlayerController
{
    #region Move
    private Vector3 moveDir;
    public float moveSpeed = 10.0f;
    #endregion

    #region Attack
    public GameObject targetObject;     // 타겟팅 표시
    public Transform targetObjectTr;
    public Vector3 targetObjectPosition;

    [SerializeField]
    Vector3 attackDir;
    // 주위의 적 객체를 저장할 콜라이더 배열
    Collider[] enemies;
    // 공격할 적 객체
    Collider attackEnemy;
    // 적 공격할 시에 받아올 에너미
    List<Collider> lEnimes;
    float attackRange = 2.6f;
    bool isAttack;
    bool targeting;
    bool isEnableCor;
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
            targetObject.SetActive(true);
            if (Vector3.Distance(tr.position, targetObject.transform.position) >= attackRange)
            {
                targetObject.transform.position = targetObject.transform.position;
            }
            else
            {
                targetObject.transform.position = tr.position + (attackDir) * 2.0f;
            }
        }
    }
   
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }

    // 짧게 누를 시에 공격
    public void ButtonAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("button Attack");
            anim.SetTrigger("Attack");

            // 가장 가까운 거리의 적 찾기
            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
            if (attackEnemy == null) return;

            TurnToEnemy(attackEnemy);
            Damage(attackEnemy);
        }
    }

    public void JoystickAttack(InputAction.CallbackContext context)
    {
        Vector2 inputVec = context.ReadValue<Vector2>();
        // 이미 타겟팅 중인 경우
        if (targeting)
        {
            attackDir = new Vector3(inputVec.x, 0, inputVec.y);
        }
        else
        {
            if (context.started)
            {
                targetObject.transform.position = tr.position;
                targeting = true;
            }
        }
        if (context.canceled)
        {
            targeting = false;
            attackEnemy = targetObject.GetComponent<attackTarget>().GetClosestEnemy();
            if (attackEnemy == null) return;

            Debug.Log("Targeting Attack");
            anim.SetTrigger("Attack");
            TurnToEnemy(attackEnemy);
            Damage(attackEnemy);
            targetObject.SetActive(false);
        }
    }

    public void TurnToEnemy(Collider attackEnemy)
    {
        // 가까운 거리의 적 방향으로 캐릭터 회전
        Vector3 enemyDir = attackEnemy.gameObject.transform.position - tr.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));
    }

    public void Damage(Collider attackEnemy)
    {
        attackEnemy.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    private void OnDrawGizmos()
    {
        if (isAttack)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tr.position, attackRange);
        }
    }

    public void Hasing()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
    }
}
