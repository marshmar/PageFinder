using System;
using UnityEngine;
using System.Collections.Generic;

public class SkillContext : ScriptContext
{
    public PlayerTarget playerTarget;
    public PlayerUtils playerUtils;
    public PlayerAnim playerAnim;
    public PlayerState playerState;
    public NewPlayerSkillController playerSkillController;
}

public class SkillBehaviour : MonoBehaviour, IChargeBehaviour, ISkillBehaviour
{
    private bool skillCanceled = false;
    private bool isChargingSkill = true;
    private Vector3 skillDir;
    private Vector3 skillSpawnPos;

    private NewScriptData scriptData;
    private GameObject skillObject;
    private SkillData skillData;
    private Collider target;
    private PlayerTarget playerTarget;
    private PlayerUtils playerUtils;
    private PlayerAnim playerAnim;
    private PlayerState playerState;
    private NewPlayerSkillController playerSkillController;
    public bool CanExcuteBehaviour()
    {
        if (playerState.CurInk < skillData.skillCost) return false;

        return true;
    }

    public void ChargingBehaviour()
    {

        switch (skillData.skillCastType)
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

    public void ExcuteBehaviour()
    {

        playerTarget.OffAllTargetObjects();

        if (!CanExcuteBehaviour()) return;

        switch (skillData.skillCastType)
        {
            case SkillCastType.DirectionBased:
                Vector3? direction = isChargingSkill ? (Vector3)skillDir : GetNearestEnemyDirection();
                CastSkill(direction);
                break;
            case SkillCastType.PositionBased:
                Vector3? spawnPos = isChargingSkill ? skillSpawnPos : GetNearestEnemyDirection();
                CastSkill(spawnPos);
                break;
        }

        playerSkillController.IsChargingSkill = false;
        skillCanceled = false;
    }

    private void SetCastPosition()
    {
        // 13: Ground Layer;
        int targetLayer = 1 << 13;
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, targetLayer))
        {
            Vector3 targetPos = new Vector3(hit.point.x, 1f, hit.point.z);
            Vector3 playerPos = playerUtils.Tr.position;
            Vector3 direction = (targetPos - playerPos).normalized;

            float distance = Vector3.Distance(playerPos, targetPos);
            float clampedDistance = Mathf.Min(distance, skillData.skillDist);

            skillSpawnPos = playerPos + direction * clampedDistance;
        }
    }

    private void SetCastDirection()
    {
        Vector2 screenDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(playerUtils.Tr.position);
        skillDir = new Vector3(screenDirection.x, 0f, screenDirection.y).normalized;
    }

    private void ShowSkillTargetingPreview()
    {
        switch (skillData.skillShapeType)
        {
            case SkillShapeType.Fan:
                if (skillData is FanSkillData fanSkillData)
                {
                    playerTarget.FanTargeting(skillDir, fanSkillData.skillRange, fanSkillData.fanDegree);
                }
                break;
            case SkillShapeType.Circle:
                playerTarget.CircleTargeting(skillSpawnPos, skillData.skillDist, skillData.skillRange);
                break;
        }
    }

    private bool CastSkill(Vector3? inputVector = null)
    {

        if (inputVector == null) return false;

        // === 애니메이션 및 방향 설정 ===
        playerAnim.ResetAnim();
        playerAnim.SetAnimationTrigger("TurningSkill");


        GameObject instantiatedSkill = null;

        switch (skillData.skillCastType)
        {
            case SkillCastType.DirectionBased:
                instantiatedSkill = Instantiate(skillObject, playerUtils.Tr.position, Quaternion.identity);
                playerUtils.TurnToDirection(inputVector.Value);
                break;
            case SkillCastType.PositionBased:
                instantiatedSkill = Instantiate(skillObject, inputVector.Value, Quaternion.identity);
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

        skillComponent.SkillInkType = scriptData.inkType;

        switch (skillData.skillCastType)
        {
            case SkillCastType.DirectionBased:
                skillComponent.ActiveSkill(inputVector.Value.normalized);
                break;
            case SkillCastType.PositionBased:
                skillComponent.ActiveSkill();
                break;
        }

        //ApplySkillModifiers(skillComponent);

        // === 잉크 소모 및 이벤트 알림 ===
        playerState.CurInk -= skillData.skillCost;
        EventManager.Instance.PostNotification(EVENT_TYPE.Skill_Successly_Used, this);

        return true;
    }

    public void GenerateInkMark(Vector3 position)
    {
        //throw new System.NotImplementedException();
    }

    public void SetContext(ScriptContext context)
    {
        SkillContext skillContext = context as SkillContext;
        if (skillContext != null)
        {
            this.playerAnim = skillContext.playerAnim;
            this.playerState = skillContext.playerState;
            this.playerTarget = skillContext.playerTarget;
            this.playerUtils = skillContext.playerUtils;
            this.playerSkillController = skillContext.playerSkillController;
        }
    }

    public void SetScriptData(NewScriptData scriptData)
    {
        this.scriptData = scriptData;
    }

    private Vector3? GetNearestEnemyDirection()
    {
        target = Utils.FindMinDistanceObject(playerUtils.Tr.position, skillData.skillDist, 1 << 6);
        return target != null ? (target.transform.position - playerUtils.Tr.position) : null;
    }

    public bool ChangeSkill(string skillName)
    {
        skillObject = SkillManager.Instance.GetSkillPrefab(skillName);
        if (skillObject == null)
        {
            return false;
        }

        // Todo: 현재 원본이 바뀌고 있음, 복사본 생성 코드로 변경 필요
        skillData = SkillManager.Instance.GetSkillData(skillName);
        if (skillData == null)
        {
            return false;
        }

        return true;
    }

    public void ExcuteAnim()
    {
        throw new NotImplementedException();
    }
}
