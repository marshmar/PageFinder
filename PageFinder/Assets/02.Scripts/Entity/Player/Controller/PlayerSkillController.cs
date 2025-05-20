using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillController : MonoBehaviour, IListener
{
    private string currSkillName;
    private SkillManager skillManager;
    private GameObject currSkillObject;
    private SkillData currSkillData;
    private PlayerAttackController playerAttackControllerScr;
    private PlayerDashController playerDashController;
    //private PlayerInkMagicController playerInkMagicControllerScr;
    // 스킬 소환 벡터
    private Vector3 spawnVector;
    // 스킬 사용중인지
    private bool isUsingSkill;
    // 타겟팅 중인지
    private bool isOnTargeting;

    // 공격할 적 객체
    private Collider attackEnemy;

    private PlayerAnim playerAnim;
    private PlayerState playerState;
    private PlayerUtils playerUtils;
    private PlayerInkType playerInkType;
    private PlayerInputAction input;
    private PlayerTarget playerTarget;
    private PlayerInteraction playerInteraction;
    private PlayerInputInvoker playerInputInvoker;
    private PlayerMoveController playerMoveController;
    public bool IsUsingSkill { get => isUsingSkill; set => isUsingSkill = value; }
    public bool IsOnTargeting { get => isOnTargeting; set => isOnTargeting = value; }
    public string CurrSkillName { get => currSkillName; set => currSkillName = value; }
    public SkillData CurrSkillData { get => currSkillData; set => currSkillData = value; }
    public bool IsChargingSkill { get => isChargingSkill; set => isChargingSkill = value; }

    public bool fireWork = false;
    public float fireWorkValue = 0;

    private Vector3 skillDir;
    private Vector3 skillSpawnPos;

    private bool isChargingSkill = false;
    private bool skillCanceled = false;
    public void Awake()
    {
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(this.gameObject, "PlayerInkType");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerTarget = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");
        playerDashController = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");
        playerInteraction = DebugUtils.GetComponentWithErrorLogging<PlayerInteraction>(this.gameObject, "PlayerInteraction");
        playerMoveController = DebugUtils.GetComponentWithErrorLogging<PlayerMoveController>(this.gameObject, "PlayerMoveController");

        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
        playerInputInvoker = DebugUtils.GetComponentWithErrorLogging<PlayerInputInvoker>(this.gameObject, "PlayerInputInvoker");
        isUsingSkill = false;
    }
    // Start is called before the first frame update
    public void Start()
    {
        skillManager = SkillManager.Instance;
        currSkillName = "SkillBulletFan";
        //currSkillName = "InkSkillEvolved";
        ChangeSkill(currSkillName);

        SetSkillAction();
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
    }

    private void SetSkillAction()
    {
        if (input is null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (input.SkillAction is null)
        {
            Debug.LogError("Skill Action이 존재하지 않습니다.");
            return;
        }

        input.SkillAction.started += context => 
        {
            skillDir = Vector3.zero;
        };

        input.SkillAction.performed += context =>
        {
            if(CheckSkillExcutable())
                isChargingSkill = true;
        };

        input.SkillAction.canceled += context =>
        {
            SkillCommand skillCommand = new SkillCommand(this, Time.time);
            playerInputInvoker.AddInputCommand(skillCommand);
        };

        if(input.CancelAction is null)
        {
            Debug.LogError("Cancel Action이 존재하지 않습니다.");
            return;
        }

        input.CancelAction.started += context =>
        {
            playerTarget.OffAllTargetObjects();
            isChargingSkill = false;
            skillCanceled = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (isChargingSkill)
        {
            switch (currSkillData.skillCastType)
            {
                case SkillCastType.DirectionBased:
                    SetCastDirection();
                    break;
                case SkillCastType.PositionBased:
                    SetCastPosition();
                    break;
            }

            ShowSkillTargetingPreview();
        }

        if (isUsingSkill)
        {
            playerAnim.CheckAnimProgress(currSkillData.skillState, currSkillData.skillAnimEndTime, ref isUsingSkill);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerInkType.SkillInkType = InkType.RED;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerInkType.SkillInkType = InkType.GREEN;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            playerInkType.SkillInkType = InkType.BLUE;
        }
    }

    private void SetCastPosition()
    {
        // 13: Ground Layer;
        int targetLayer = 1 << 13;
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100f, Color.red, 1f);
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, targetLayer))
        {
            Vector3 targetPos = new Vector3(hit.point.x, 1f, hit.point.z);
            Vector3 playerPos = playerUtils.Tr.position;
            Vector3 direction = (targetPos - playerPos).normalized;

            float distance = Vector3.Distance(playerPos, targetPos);
            float clampedDistance = Mathf.Min(distance, currSkillData.skillDist);

            skillSpawnPos = playerPos + direction * clampedDistance;
            Debug.Log(skillSpawnPos);
        }
    }

    private void SetCastDirection()
    {
        Vector2 screenDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(playerUtils.Tr.position);
        skillDir = new Vector3(screenDirection.x, 0f, screenDirection.y).normalized;
    }

    private void ShowSkillTargetingPreview()
    {
        switch (currSkillData.skillShapeType)
        {
            case SkillShapeType.Fan:
                if (currSkillData is FanSkillData fanSkillData)
                {
                    playerTarget.FanTargeting(skillDir, fanSkillData.skillRange, fanSkillData.fanDegree);
                }
                break;
            case SkillShapeType.Circle:
                playerTarget.CircleTargeting(skillSpawnPos, currSkillData.skillDist, currSkillData.skillRange);
                break;
        }

    }

    public bool CheckSkillExcutable()
    {
        return !isUsingSkill && playerState.CurInk >= currSkillData.skillCost /*&& !playerAttackControllerScr.IsAttacking*/ && !playerDashController.ChargingDash 
            && !playerInteraction.IsInteractable;
    }

    public void ExcuteSkill()
    {
/*        if (!skillCanceled && CheckSkillExcutable())
        {
            playerMoveController.CanMove = true;
            playerMoveController.MoveTurn = true;

            if (!isChargingSkill)
                InstantiateSkill();
            else
                InstantiateSkill(playerUtils.Tr.position + skillDir);
        }*/

        switch (currSkillData.skillCastType)
        {
            case SkillCastType.DirectionBased:
                Vector3? direction = isChargingSkill ?/* playerUtils.Tr.position + */(Vector3)skillDir : GetNearestEnemyDirection();
                CastSkill(direction);
                break;
            case SkillCastType.PositionBased:
                Vector3? spawnPos = isChargingSkill ? skillSpawnPos : GetNearestEnemyDirection();
                CastSkill(spawnPos);
                break;
        }

        
        playerTarget.OffAllTargetObjects();
        skillCanceled = false;
        isChargingSkill = false;
    }

    

    /// <summary>
    /// 장착된 스킬을 변경하는 함수
    /// </summary>
    /// <param name="skillName">변경할 스킬 이름</param>
    public bool ChangeSkill(string skillName)
    {
        if(!DebugUtils.CheckIsNullWithErrorLogging<SkillManager>(skillManager, this.gameObject))
        {
            this.currSkillObject = skillManager.GetSkillPrefab(skillName);
            if(DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                return false;
            }
            this.currSkillData = skillManager.GetSkillData(skillName);
            if (DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
            {
                return false;
            }
            this.currSkillData.skillInkType = playerInkType.SkillInkType;
        }
        return true;
    }



    public IEnumerator FireWork()
    {
        playerState.CurAttackSpeed.AddModifier(new StatModifier(0.1f, StatModifierType.PercentAddTemporary, this));
        //playerState.CurAttackSpeed += 0.1f;
        yield return new WaitForSeconds(3.0f);

        playerState.CurAttackSpeed.RemoveAllFromSource(this);
        //playerState.CurAttackSpeed -= 0.1f;
    }

    public bool CastSkill(Vector3? inputVector = null)
    {
        if (!CheckSkillExcutable())
            return false;

        if (inputVector == null) return false;

        // === 애니메이션 및 방향 설정 ===
        playerAnim.ResetAnim();
        playerAnim.SetAnimationTrigger("TurningSkill");


        GameObject instantiatedSkill = null;

        switch (currSkillData.skillCastType)
        {
            case SkillCastType.DirectionBased:
                instantiatedSkill = Instantiate(currSkillObject, playerUtils.Tr.position, Quaternion.identity);
                playerUtils.TurnToDirection(inputVector.Value);
                break;
            case SkillCastType.PositionBased:
                instantiatedSkill = Instantiate(currSkillObject, inputVector.Value, Quaternion.identity);
                playerUtils.TurnToDirection(inputVector.Value - playerUtils.Tr.position);
                break;
        }

        // === 스킬 프리팹 소환 ===
        if (instantiatedSkill == null)
        {
            Debug.LogWarning("스킬 프리팹 인스턴스화 실패.");
            return false;
        }

        // === 스킬 로직 적용 ===
        Skill skillComponent = instantiatedSkill.GetComponent<Skill>();
        if (skillComponent == null)
        {
            Debug.LogWarning("스킬 컴포넌트가 존재하지 않습니다.");
            return false;
        }

        skillComponent.SkillInkType = playerInkType.SkillInkType;

        switch (currSkillData.skillCastType)
        {
            case SkillCastType.DirectionBased:
                skillComponent.ActiveSkill(inputVector.Value.normalized);
                break;
            case SkillCastType.PositionBased:
                skillComponent.ActiveSkill();
                break;
        }

        ApplySkillModifiers(skillComponent);

        // === 잉크 소모 및 이벤트 알림 ===
        playerState.CurInk -= currSkillData.skillCost;
        EventManager.Instance.PostNotification(EVENT_TYPE.Skill_Successly_Used, this);

        return true;
    }

    private void ApplySkillModifiers(Skill skill)
    {
        skill.SkillInkType = playerInkType.SkillInkType;

        if (skill is BulletFanSkill bulletFanSkill && fireWork)
        {
            bulletFanSkill.bulletSpeed *= (1 + fireWorkValue);
        }
    }

    private Vector3? GetNearestEnemyDirection()
    {
        attackEnemy = Utils.FindMinDistanceObject(playerUtils.Tr.position, currSkillData.skillDist, 1 << 6);
        return attackEnemy != null ? (attackEnemy.transform.position - playerUtils.Tr.position) : null;
    }


    /// <summary>
    /// 가장 가까운 적에게 스킬을 소환하는 함수
    /// </summary>
    /// <return>스킬 소환 성공 여부</return>
    public bool InstantiateSkill()
    {
/*        if (!isUsingSkill && playerState.CurInk >= currSkillData.skillCost *//*&& !playerAttackControllerScr.IsAttacking*//*)
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                { 
                    switch (currSkillData.skillType)
                    {
                        case SkillTypes.FAN:
                            attackEnemy = Utils.FindMinDistanceObject(playerUtils.Tr.position, currSkillData.skillDist, 1 << 6);
                            if (!DebugUtils.CheckIsNullWithErrorLogging(attackEnemy, "공격할 적 객체가 없습니다."))
                            {
                                isUsingSkill = true;

                                GameObject instantiatedSkill = Instantiate(currSkillObject, playerUtils.Tr.position, Quaternion.identity);
                                if (!DebugUtils.CheckIsNullWithErrorLogging(instantiatedSkill, this.gameObject))
                                {
                                    playerAnim.ResetAnim();
                                    playerAnim.SetAnimationTrigger("TurningSkill");
                                    if (attackEnemy.transform.position == null) return false;
                                    spawnVector = attackEnemy.transform.position - playerUtils.Tr.position;
                                    playerUtils.TurnToDirection(spawnVector);
                                    Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                    if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                    {
                                        if(skill is BulletFanSkill bulletFanSkill)
                                        {
                                            if (fireWork) bulletFanSkill.bulletSpeed *= (1 + fireWorkValue);
                                        }
                                        skill.SkillInkType = playerInkType.SkillInkType;
                                        skill.ActiveSkill(spawnVector.normalized);
                                        playerState.CurInk -= currSkillData.skillCost;
                                        EventManager.Instance.PostNotification(EVENT_TYPE.Skill_Successly_Used, this);
                                        return true;
                                    }
                                }
                            }
                            break;
                        case SkillTypes.PAINT:
                            break;
                        default:
                            spawnVector = new Vector3(playerUtils.Tr.position.x, playerUtils.Tr.position.y + 0.1f, playerUtils.Tr.position.z);
                            break;
                    }
                }
             }
        }
        */
        return false;
    }

    // 지정한 위치에 스킬 소환하는 함수
    public bool InstantiateSkill(Vector3 pos)
    {
        /*
        if (!isUsingSkill && playerState.CurInk >= currSkillData.skillCost *//*&& !playerAttackControllerScr.IsAttacking*//*)
        {
            //rangedEntity.DisableCircleRenderer();
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                {
                    GameObject instantiatedSkill = Instantiate(currSkillObject, playerUtils.Tr.position, Quaternion.identity);
                    if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedSkill, this.gameObject))
                    {
                        switch (currSkillData.skillType)
                        {
                            case SkillTypes.FAN:
                                isUsingSkill = true;
                                playerUtils.TurnToDirection(pos);
                                playerAnim.ResetAnim();
                                playerAnim.SetAnimationTrigger("TurningSkill");
                                Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                {
                                    if (skill is BulletFanSkill bulletFanSkill)
                                    {
                                        if (fireWork) bulletFanSkill.bulletSpeed *= (1 + fireWorkValue);
                                    }
                                    skill.SkillInkType = playerInkType.SkillInkType;
                                    skill.ActiveSkill(pos.normalized);
                                    playerState.CurInk -= currSkillData.skillCost;
                                    EventManager.Instance.PostNotification(EVENT_TYPE.Skill_Successly_Used, this);
                                    return true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    */

        return false;
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        /*        switch (eventType)
                {
                    case EVENT_TYPE.Joystick_Short_Released:
                        if(sender.name == "Player_UI_OP_Skill")
                            //InstantiateSkill();
                        break;
                    case EVENT_TYPE.Joystick_Long_Released:
                        if(sender.name == "Player_UI_OP_Skill")
                        {
                            Vector3 vector = (Vector3)param;
                            //InstantiateSkill(position);
                        }
                        break;
                }*/
    }
}
