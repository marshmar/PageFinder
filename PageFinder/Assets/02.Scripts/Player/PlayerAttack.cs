using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : Player
{
    #region Attack
    public GameObject targetObject;     // 타겟팅 표시
    private Transform targetObjectTr;

    [SerializeField]
    Vector3 attackDir;
    // 공격할 적 객체
    Collider attackEnemy;

    float attackRange = 2.6f;
    float attackPlus = 2.2f;
    bool targeting;

    #endregion

    #region Skills
    public GameObject[] skillPrefabs;
    RaycastHit skillRayHit;

    bool skillMode = false;
    float skillDist = 10.0f;
    #endregion
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        targeting = false;
        attackEnemy = null;
        targetObjectTr = targetObject.GetComponent<Transform>();
        targetObject.SetActive(false);
        skillPrefabs[0].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (targeting)
        {
            targetObject.SetActive(true);
            if (Vector3.Distance(tr.position, targetObjectTr.position) >= attackRange)
            {
                targetObjectTr.position = targetObjectTr.position;
            }
            else
            {
                targetObjectTr.position = tr.position + (attackDir) * attackPlus;
            }
        }
        else
        {
            targetObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            skillMode = true;
        }

        if (skillMode)
        {
            SetSkillPos();

        }
    }

    // 짧게 누를 시에 공격
    public void ButtonAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 현재 진행중인 애니메이션이 공격 애니메이션이 아닐 때만 공격 가능
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Warrior_Attack"))
            {
                Debug.Log("button Attack");
                anim.SetTrigger("Attack");

                // 가장 가까운 거리의 적 찾기
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
                if (attackEnemy == null) return;

                TurnToDirection(CaculateDirection(attackEnemy));
                Damage(attackEnemy);
                //portal.ChangeColor(palette.ReturnCurrentColor());
            }
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
                targetObjectTr.position = tr.position;
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

            TurnToDirection(CaculateDirection(attackEnemy));
            Damage(attackEnemy);
            targetObject.SetActive(false);
        }
    }

    public void Damage(Collider attackEnemy)
    {
        attackEnemy.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void SetSkillPos()
    {
        skillPrefabs[0].SetActive(true);
        Transform skillTr = skillPrefabs[0].transform;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        skillTr.position = new Vector3(mousePos.x, 0.1f, mousePos.z);
        if (Vector3.Distance(tr.position, skillTr.position) >= skillDist)
        {
            skillTr.position = skillTr.position;
        }


        if (Input.GetMouseButtonDown(0))
        {
            skillMode = false;
            Instantiate(skillPrefabs[0], skillTr.position, tr.rotation);
            skillPrefabs[0].SetActive(false);
        }

    }
}
