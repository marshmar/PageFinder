using System;
using UnityEngine;
using System.Collections.Generic;

public class SkillContext : ScriptContext
{
    public Player player;
}

public class SkillBehaviour : MonoBehaviour, IChargeBehaviour, ISkillBehaviour
{
    private bool skillCanceled = false;
    //private bool isChargingSkill = true;
    private Vector3 skillDir;
    private Vector3 skillSpawnPos;

    private NewScriptData scriptData;
    private Collider target;
    private Player player;
    private SkillScript skillScript;


    public event Action AfterEffect;
    private Stat skillBasicDamage;

    public Stat SkillBasicDamage { get => skillBasicDamage; }

    public void SetSkillScript(SkillScript skillScript)
    {
        this.skillScript = skillScript;
        skillBasicDamage = new Stat(skillScript.SkillData.skillBasicDamage);
    }

    public bool CanExcuteBehaviour()
    {
        if (player.State.CurInk < skillScript.SkillCost) return false;

        if (player.DashController.IsDashing || player.SkillController.IsUsingSkill) return false;

        return true;
    }

    public void ChargingBehaviour()
    {

        switch (skillScript.SkillCastType)
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

        player.Target.OffAllTargetObjects();

        if (!CanExcuteBehaviour()) return;

        switch (skillScript.SkillCastType)
        {
            case SkillCastType.DirectionBased:
                Vector3? direction = player.SkillController.IsChargingSkill ? (Vector3)skillDir : GetNearestEnemyDirection();
                CastSkill(direction);
                break;
            case SkillCastType.PositionBased:
                Vector3? spawnPos = player.SkillController.IsChargingSkill ? skillSpawnPos : GetNearestEnemyPosition();
                CastSkill(spawnPos);
                break;
        }

        player.SkillController.IsChargingSkill = false;
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
            Vector3 targetPos = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
            Vector3 playerPos = player.Utils.Tr.position;
            Vector3 direction = (targetPos - playerPos).normalized;

            float distance = Vector3.Distance(playerPos, targetPos);
            float clampedDistance = Mathf.Min(distance, skillScript.SkillDist);

            skillSpawnPos = playerPos + direction * clampedDistance;
        }
    }

    private void SetCastDirection()
    {
        Vector2 screenDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(player.Utils.Tr.position);
        skillDir = new Vector3(screenDirection.x, 0f, screenDirection.y).normalized;
    }

    private void ShowSkillTargetingPreview()
    {
        switch (skillScript.SkillShapeType)
        {
            case SkillShapeType.Fan:
                if (skillScript.SkillData is FanSkillData fanSkillData)
                {
                    player.Target.FanTargeting(skillDir, fanSkillData.skillRange, fanSkillData.fanDegree);
                }
                break;
            case SkillShapeType.Circle:
                player.Target.CircleTargeting(skillSpawnPos, skillScript.SkillDist, skillScript.SkillRange);
                break;
        }
    }

    private bool CastSkill(Vector3? inputVector = null)
    {

        if (inputVector == null) return false;

        // === 애니메이션 및 방향 설정 ===
        player.Anim.ResetAnim();
        player.Anim.SetAnimationTrigger("TurningSkill");

        player.SkillController.IsUsingSkill = true;

        GameObject instantiatedSkill = null;

        switch (skillScript.SkillCastType)
        {
            case SkillCastType.DirectionBased:
                instantiatedSkill = Instantiate(skillScript.SkillObject, player.Utils.Tr.position, Quaternion.identity);
                player.Utils.TurnToDirection(inputVector.Value);
                break;
            case SkillCastType.PositionBased:
                instantiatedSkill = Instantiate(skillScript.SkillObject, inputVector.Value, Quaternion.identity);
                player.Utils.TurnToDirection(inputVector.Value - player.Utils.Tr.position);
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
        skillComponent.SkillBasicDamage = this.skillBasicDamage;

        switch (skillScript.SkillCastType)
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
        player.State.CurInk -= skillScript.SkillCost;
        EventManager.Instance.PostNotification(EVENT_TYPE.Skill_Successly_Used, this);
        EventManager.Instance.PostNotification(EVENT_TYPE.FirstInkSkill, this);
        AfterEffect?.Invoke();
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
            player = skillContext.player;  
        }
    }

    public void SetScriptData(NewScriptData scriptData)
    {
        this.scriptData = scriptData;
    }

    private Vector3? GetNearestEnemyDirection()
    {
        target = Utils.FindMinDistanceObject(player.Utils.Tr.position, skillScript.SkillDist, 1 << 6);
        return target != null ? (target.transform.position - player.Utils.Tr.position) : null;
    }

    private Vector3? GetNearestEnemyPosition()
    {
        if (target == null) return null;

        target = Utils.FindMinDistanceObject(player.Utils.Tr.position, skillScript.SkillDist, 1 << 6);
        // 13: Ground Layer;
        int targetLayer = LayerMask.GetMask("GROUND");
        Ray groundRay = new Ray(target.transform.position, Vector3.down);
        RaycastHit hit;
        Vector3 skillSpawnPos = target.transform.position;
        if (Physics.Raycast(groundRay, out hit, Mathf.Infinity, targetLayer))
        {
            skillSpawnPos = hit.point + new Vector3(0f, 0.1f, 0f);
        }
        return target != null ? skillSpawnPos : null;
    }

    public void ExcuteAnim()
    {
        throw new NotImplementedException();
    }
}
