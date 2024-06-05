using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : Player
{
    private Enemy enemyScr;
    private float maxTapDuration = 0.3f;
    #region Attack
    public GameObject targetObject;     
    private Transform targetObjectTr;

    [SerializeField]
    private VirtualJoystick attackJoystick;
    private VirtualJoystick skillJoystickOne;
    private VirtualJoystick skillJoystickTwo;

    [SerializeField]
    private Vector3 attackDir;
    // 공격할 적 객체
    Collider attackEnemy;

    private float attackRange = 2.6f;
    private float touchStartTime;
    private bool targeting;


    #endregion

    #region Skills
    public GameObject[] skillPrefabs;
    public GameObject[] skillBackgroundObjs;
    public GameObject[] skillJoySticks;

    bool skillTargeting = false;
    float skillDist = 5.0f;

    #endregion
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        targeting = false;
        attackEnemy = null;
        targetObjectTr = targetObject.GetComponent<Transform>();
        targetObject.SetActive(false);

        BaseSetting();
    }

    // Update is called once per frame
    void Update()
    {
        /*        // 일반 공격 시
                if (targeting)
                    OnTargeting(attackRange);
                // 스킬 공격 시
                else if (skillTargeting)
                    OnTargeting(skillDist);
                else
                    targetObject.SetActive(false);*/


        if (attackJoystick.IsTouched)
        {
            float x = attackJoystick.Horizontal();
            float y = attackJoystick.Vertical();
            attackDir = new Vector3(x, 0, y);
            Debug.Log(attackJoystick.GetTouchDuration());
            Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack"));
            if (attackJoystick.GetTouchDuration() < 1.0f && !anim.GetCurrentAnimatorStateInfo(0).IsName("player_attack"))
            {
                anim.SetTrigger("Attack");
                Debug.Log("짧은 공격");
            }
        }

    }

    private void BaseSetting()
    {
        foreach(GameObject i in skillBackgroundObjs)
        {
            i.SetActive(false);
        }
    }
    /// <summary>
    /// 타겟팅 객체 움직이기
    /// </summary>
    /// <param name="targetingRange">스킬 범위</param>
    public void OnTargeting(float targetingRange)
    {
        targetObject.SetActive(true);
        // 사거리를 벗어날 경우 제자리 고정
        if (Vector3.Distance(tr.position, targetObjectTr.position) >= targetingRange)
        {
            targetObjectTr.position = targetObjectTr.position;
        }
        // 타겟팅 오브젝트 움직이기
        else
        {
            targetObjectTr.position = (tr.position + (attackDir) * (targetingRange-0.1f));
        }
    }

    // 공격 버튼 짧게 누를 시에 공격 매커니즘(범위내에 가장 가까운 적을 공격)
    public void ButtonAttack(InputAction.CallbackContext context)
    {
        // 버튼 입력 시작 시 입력 시간 측정
        if(context.started)
            touchStartTime = Time.time;
        if (context.canceled)
        {
            // 입력 시간이 0.3초 이상일 경우 조이스틱 공격으로 전환
            if (Time.time - touchStartTime >= maxTapDuration) return;
            touchStartTime = 0f;
            anim.SetTrigger("Attack");


            // 범위 내에서 가장 가까운 적 찾기(없을 경우 공격 모션만)
            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
            Debug.Log(attackEnemy);
            if (attackEnemy == null) return;


            // 적 방향으로 플레이어 회전
            TurnToDirection(CaculateDirection(attackEnemy));
            Damage(attackEnemy);
        }
    }

    // 공격 버튼 홀드 시(타겟팅 오브젝트를 움직여 공격 대상 설정)
    public void JoystickAttack(InputAction.CallbackContext context)
    {
        // 조이스틱 입력
        Vector2 inputVec = context.ReadValue<Vector2>();

        // 타겟팅 상황일 경우 공격 방향 벡터만 설정
        if (targeting)
        {
            attackDir = new Vector3(inputVec.x, 0, inputVec.y);
        }
        // 타겟팅 상황이 아닐 경우
        else
        {
            if (context.started)
            {
                targetObjectTr.position = tr.position;
                targeting = true;
            }
            
        }
        // 조이스틱 입력 종료 시
        if (context.canceled)
        {
            targeting = false;
            attackEnemy = targetObject.GetComponent<attackTarget>().GetClosestEnemy();
            if (attackEnemy == null) return;

            anim.SetTrigger("Attack");

            // 적 객체 방향으로 플레이어 회전
            TurnToDirection(CaculateDirection(attackEnemy));
            Damage(attackEnemy);
            targetObject.SetActive(false);
        }
    }

    // 스킬 버튼 짧게 클릭 시(가장 가까운 적의 방향으로 스킬 사용)
    public void ButtonSkill(InputAction.CallbackContext context)
    {
        if (context.started)
            touchStartTime = Time.time;
        // 
        if (context.canceled)
        {
            // 입력 시간이 0.3초 이상일 경우 조이스틱 스킬로 전환
            if (Time.time - touchStartTime >= maxTapDuration) return;

            anim.SetTrigger("SpawnSkill");
            // 가장 가까운 적 객체 찾기
            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, skillDist, 1 << 6);
            if (attackEnemy == null) return;

            // 찾은 적 객체에 스킬 좌표 설정 및 소환
            InstantiateSkill(attackEnemy.transform.position, skillPrefabs[0]);

            TurnToDirection(attackDir);
        }
    }

    // 
    public void JoystickSkill(InputAction.CallbackContext context)
    {
        Vector2 inputVec = context.ReadValue<Vector2>();
        // 스킬 타겟팅 중일 경우 방향만 설정
        if (skillTargeting)
        {
            attackDir = new Vector3(inputVec.x, 0, inputVec.y);
        }
        else
        {
            // 공격 시작할 경우
            if (context.started)
            {
                skillBackgroundObjs[0].SetActive(true);
                targetObjectTr.position = tr.position;
                skillTargeting = true;
            }
        }
        // 조이스틱 입력 종료 시
        if (context.canceled)
        {
            skillBackgroundObjs[0].SetActive(false);
            anim.SetTrigger("SpawnSkill");
            skillTargeting = false;
            InstantiateSkill(targetObject.transform.position, skillPrefabs[0]);
            Debug.Log(attackDir);
            TurnToDirection(attackDir);
        }
    }


    // 2번 스킬 버튼
    public void ButtonSkillTwo(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            touchStartTime = Time.time;
        }


        // 버튼 입력 종료 시
        if (context.canceled)
        {
            float touchDuration = Time.time - touchStartTime;
            if (touchDuration >= maxTapDuration) return;

            anim.SetTrigger("TurningSkill");
            
            InstantiateSkill((tr.position + (tr.forward) * 3.0f), skillPrefabs[1]);
        }
    }

    // 데미지 입히는 함수
    public void Damage(Collider attackEnemy)
    {
        enemyScr = attackEnemy.gameObject.GetComponent<Enemy>();
        enemyScr.HP -= atk;
    }

    // 지정한 위치에 스킬 소환하는 함수
    public void InstantiateSkill(Vector3 pos, GameObject skillObject)
    {
        if (skillObject == null) Debug.LogError("소환할 스킬 오브젝트가 없습니다.");
        Instantiate(skillObject, pos, Quaternion.identity);
    }

    /*public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(tr.position, attackRange);
    }*/
}
