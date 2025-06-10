using System;
using System.Collections;
using UnityEngine;

public class BasicAttackContext : ScriptContext
{
    public Player player;
    public GameObject[] baEffectRed;
    public GameObject[] baEffectGreen;
    public GameObject[] baEffectBlue;
}

public class BasicAttackBehaviour : MonoBehaviour, IScriptBehaviour
{
    private float attackAnimLength;
    private Collider target;
    private Player player;
    private NewScriptData scriptData;
    private GameObject attackEffect;
    private Coroutine attackCoroutine;

    private GameObject[] baEffectRed;
    private GameObject[] baEffectGreen;
    private GameObject[] baEffectBlue;

    public event Action AfterEffect;

    public bool CanExcuteBehaviour()
    {
        if (player.AttackController.IsAttacking && !player.Anim.HasAttackAnimPassedTime(0.8f))
            return false;

        if (player.AttackController.IsNextAttackBuffered)
            return false;

        if (player.DashController.IsDashing || player.SkillController.IsUsingSkill)
            return false;

        if(player.AttackController.IsAttacking && player.Anim.HasAttackAnimPassedTime(0.8f))
        {
            player.AttackController.IsNextAttackBuffered = true;
        }

        return true;
    }

    public void ExcuteBehaviour()
    {
        player.AttackController.IsAttacking = true;

        // 에너미 찾기
        FindTarget();

        // 기본공격 범위 보여주기
        ShowAttackRange();

        // 에너미 존재시 타켓 마커 활성화 및 플레이어 회전
        if(target != null)
        {
            ShowTargetMarker();
            RotateToTarget();
        }

        // 애니메이션 재생
        //PlayAttackAnim();

        SetBaInk();

        // 공격 오브젝트 움직이기
        SweepArkAttackEachComboStep();

        // 사운드 재생
        PlayAudio();

        // 이팩트 재생
        GenerateEffect();

        // 콤보 증가
        IncreaseCombo();

        AfterEffect?.Invoke();
    }

    private void SetBaInk()
    {
        player.BasicAttackCollider.baInkType = scriptData.inkType;
    }

    public void StopBehaviour()
    {
        if(attackCoroutine != null)
        {
            CoroutineRunner.Instance.StopRunningCoroutine(attackCoroutine);
            attackCoroutine = null;
            player.BasicAttackCollider.gameObject.SetActive(false);
        }

        if(attackEffect != null)
        {
            Destroy(attackEffect);
            attackEffect = null;
        }
    }


    private void FindTarget()
    {
        if(player.State == null)
        {
            Debug.LogError("PlayerState is null");
            return;
        }

        if(player.Utils == null)
        {
            Debug.LogError("PlayerUtils is null");
            return;
        }

        // 6: Enemy Layer, 11: Interactive Object Layer
        int targetLayer = (1 << 6) + (1 << 11);

#if UNITY_STANDALONE
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, targetLayer))
        {
            Collider primaryTarget = hit.collider;
            if (Vector3.Distance(player.Utils.Tr.position, primaryTarget.transform.position) <= player.State.CurAttackRange.Value)
            {
                target = primaryTarget;
                return;
            }
        }
#endif 
        if(target != null)
        {
            if(Vector3.Distance(target.transform.position, player.Utils.Tr.position) <= player.State.CurAttackRange.Value)
            {
                return;
            }
        }

        target = Utils.FindMinDistanceObject(player.Utils.Tr.position, player.State.CurAttackRange.Value, targetLayer);
    }

    private void ShowAttackRange()
    {
        if (player.Target == null)
        {
            Debug.LogError("PlayerTarget is null");
            return;
        }
        player.Target.CircleRangeOn(player.State.CurAttackRange.Value, 0.1f);
    }


    private void ShowTargetMarker()
    {
        if(player.TargetMarker == null)
        {
            Debug.LogError("TargetMarker is null");
            return;
        }

        player.TargetMarker.IsActive = true;
        player.TargetMarker.TargetTransform = target.transform;
    }


    private void RotateToTarget()
    {
        if(player.Utils == null)
        {
            Debug.Log("PlayerUtils is null");
            return;
        }

        Vector3 dirToTarget = player.Utils.CalculateDirectionFromPlayer(target);
        player.Utils.TurnToDirection(dirToTarget);
    }

    private void PlayAttackAnim()
    {
        //playerAnim.SetAnimationTrigger("Attack");
    }

    private void SweepArkAttackEachComboStep()
    {
        switch (player.AttackController.ComboCount)
        {
            case 0:
                attackCoroutine = CoroutineRunner.Instance.RunCoroutine(SweepArkAttack(-45.0f, 90.0f));
                break;
            case 1:
                attackCoroutine = CoroutineRunner.Instance.RunCoroutine(SweepArkAttack(45.0f, -90.0f));
                break;
            case 2:
                attackCoroutine = CoroutineRunner.Instance.RunCoroutine(SweepArkAttack(-70.0f, 140.0f));
                break;
        }
    }

    private IEnumerator SweepArkAttack(float startDegree, float degreeAmount)
    {
        if(player.BasicAttackCollider == null)
        {
            Debug.LogError("PlayerBasicAttackCollider is null");
            yield break;
        }

        if(player.Anim == null)
        {
            Debug.LogError("PlayerAnim is null");
            yield break;
        }

        GameObject attackObj = player.BasicAttackCollider.gameObject;

        attackObj.SetActive(true);
        attackObj.transform.localPosition = Vector3.zero;

        float attackTime = 0;
        float currDegree = startDegree;
        float targetDegree = startDegree + degreeAmount;

        attackObj.transform.rotation = Quaternion.Euler(0, player.Utils.ModelTr.rotation.eulerAngles.y + startDegree, 0);
        //attackAnimLength = player.Anim.GetCurrAnimLength() * 0.75f;
        attackAnimLength = player.Anim.GetAdjustedAnimDuration() * 0.75f;

        while (attackTime <= attackAnimLength * 0.4f)
        {
            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / (attackAnimLength * 0.4f));

            attackObj.transform.rotation = Quaternion.Euler(0, player.Utils.ModelTr.rotation.eulerAngles.y + currDegree, 0);

            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, player.Utils.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        attackObj.SetActive(false);

        attackCoroutine = null;
        yield break;
    }

    public void GenerateInkMark(Vector3 position)
    {
        
    }

    public void GenerateEffect()
    {
        attackEffect = null;
        switch (scriptData.inkType)
        {
            case InkType.RED:
                attackEffect = Instantiate(baEffectRed[player.AttackController.ComboCount]);
                break;
            case InkType.GREEN:
                attackEffect = Instantiate(baEffectGreen[player.AttackController.ComboCount]);
                break;
            case InkType.BLUE:
                attackEffect = Instantiate(baEffectBlue[player.AttackController.ComboCount]);
                break;

        }

        // 0.5f is distance offset
        attackEffect.transform.position = player.Utils.Tr.position - (0.5f * player.Utils.ModelTr.forward);
        attackEffect.transform.rotation = Quaternion.Euler(attackEffect.transform.rotation.eulerAngles.x, player.Utils.ModelTr.eulerAngles.y, 180f);

        Destroy(attackEffect, attackAnimLength * 0.7f);
    }

    private void PlayAudio()
    {
        switch (player.AttackController.ComboCount)
        {
            case 0:
                AudioManager.Instance.Play(Sound.attack1Sfx, AudioClipType.BaSfx);
                break;
            case 1:
                AudioManager.Instance.Play(Sound.attack2Sfx, AudioClipType.BaSfx);
                break;
            case 2:
                AudioManager.Instance.Play(Sound.attack3Sfx, AudioClipType.BaSfx);
                break;
        }
    }

    public void SetContext(ScriptContext context)
    {
        BasicAttackContext baContext = context as BasicAttackContext;
        if(baContext == null)
        {
            Debug.LogError("Failed to Convert BasicAttackContext");
            return;
        }
        player = baContext.player;
        baEffectRed = baContext.baEffectRed;
        baEffectGreen = baContext.baEffectGreen;
        baEffectBlue = baContext.baEffectBlue;
    }

    public void SetScriptData(NewScriptData scriptData)
    {
        this.scriptData = scriptData;
    }

    private void IncreaseCombo()
    {
        player.AttackController.ComboCount += 1;
        if(player.AttackController.ComboCount >= 3)
        {
            player.AttackController.ComboCount = 0;
        }
    }

    public void ExcuteAnim()
    {
        player.Anim.SetAnimationTrigger("Attack");
    }

    public void SetDamageMultiplier(float amount)
    {
        player.BasicAttackCollider.DamageMultiplier = amount;
    }
}
