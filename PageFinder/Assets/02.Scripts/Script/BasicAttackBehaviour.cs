using System;
using System.Collections;
using UnityEngine;

public class BasicAttackContext : ScriptContext
{
    public PlayerUtils playerUtils;
    public PlayerState playerState;
    public PlayerTarget playerTarget;
    public PlayerAnim playerAnim;
    public TargetObject targetMarker;
    public PlayerBasicAttackCollider basicAttackCollider;
    public NewPlayerAttackController playerAttackController;
    public GameObject[] baEffectRed;
    public GameObject[] baEffectGreen;
    public GameObject[] baEffectBlue;
}

public class BasicAttackBehaviour : MonoBehaviour, IScriptBehaviour
{
    private float attackAnimLength;
    private Collider target;
    private PlayerUtils playerUtils;
    private PlayerState playerState;
    private PlayerTarget playerTarget;
    private PlayerAnim playerAnim;
    private TargetObject targetMarker;
    private PlayerBasicAttackCollider basicAttackCollider;
    private NewPlayerAttackController playerAttackController;
    private NewScriptData scriptData;
    private GameObject attackEffect;
    private Coroutine attackCoroutine;

    private GameObject[] baEffectRed;
    private GameObject[] baEffectGreen;
    private GameObject[] baEffectBlue;

    public bool CanExcuteBehaviour()
    {
        if (playerAttackController.IsAttacking && !playerAnim.GetAttackAnimProcessOverPercent(0.8f))
            return false;

        if (playerAttackController.IsNextAttackBuffered)
            return false;
        
        if(playerAttackController.IsAttacking && playerAnim.GetAttackAnimProcessOverPercent(0.8f))
        {
            playerAttackController.IsNextAttackBuffered = true;
        }

        return true;
    }

    public void ExcuteBehaviour()
    {
        playerAttackController.IsAttacking = true;

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

        // 공격 오브젝트 움직이기
        SweepArkAttackEachComboStep();

        // 사운드 재생
        PlayAudio();

        // 이팩트 재생
        GenerateEffect();

        // 콤보 증가
        IncreaseCombo();
    }

    public void StopBehaviour()
    {
        if(attackCoroutine != null)
        {
            CoroutineRunner.Instance.StopRunningCoroutine(attackCoroutine);
            attackCoroutine = null;
            basicAttackCollider.gameObject.SetActive(false);
        }

        if(attackEffect != null)
        {
            Destroy(attackEffect);
            attackEffect = null;
        }
    }


    private void FindTarget()
    {
        if(playerState == null)
        {
            Debug.LogError("PlayerState is null");
            return;
        }

        if(playerUtils == null)
        {
            Debug.LogError("PlayerUtils is null");
            return;
        }

        // 6: Enemy Layer, 11: Interactive Object Layer
        int targetLayer = (1 << 6) + (1 << 1);

#if UNITY_STANDALONE
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, targetLayer))
        {
            Collider primaryTarget = hit.collider;
            if (Vector3.Distance(playerUtils.Tr.position, primaryTarget.transform.position) <= playerState.CurAttackRange.Value)
            {
                target = primaryTarget;
                return;
            }
        }
#endif 
        if(target != null)
        {
            if(Vector3.Distance(target.transform.position, playerUtils.Tr.position) <= playerState.CurAttackRange.Value)
            {
                return;
            }
        }

        target = Utils.FindMinDistanceObject(playerUtils.Tr.position, playerState.CurAttackRange.Value, targetLayer);
    }

    private void ShowAttackRange()
    {
        if (playerTarget == null)
        {
            Debug.LogError("PlayerTarget is null");
            return;
        }
        playerTarget.CircleRangeOn(playerState.CurAttackRange.Value, 0.1f);
    }


    private void ShowTargetMarker()
    {
        if(targetMarker == null)
        {
            Debug.LogError("TargetMarker is null");
            return;
        }

        targetMarker.IsActive = true;
        targetMarker.TargetTransform = target.transform;
    }


    private void RotateToTarget()
    {
        if(playerUtils == null)
        {
            Debug.Log("PlayerUtils is null");
            return;
        }

        Vector3 dirToTarget = playerUtils.CalculateDirectionFromPlayer(target);
        playerUtils.TurnToDirection(dirToTarget);
    }

    private void PlayAttackAnim()
    {
        //playerAnim.SetAnimationTrigger("Attack");
    }

    private void SweepArkAttackEachComboStep()
    {
        switch (playerAttackController.ComboCount)
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
        if(basicAttackCollider == null)
        {
            Debug.LogError("PlayerBasicAttackCollider is null");
            yield break;
        }

        if(playerAnim == null)
        {
            Debug.LogError("PlayerAnim is null");
            yield break;
        }

        GameObject attackObj = basicAttackCollider.gameObject;

        attackObj.SetActive(true);
        attackObj.transform.localPosition = Vector3.zero;

        float attackTime = 0;
        float currDegree = startDegree;
        float targetDegree = startDegree + degreeAmount;

        attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + startDegree, 0);
        attackAnimLength = playerAnim.GetCurrAnimLength() * 0.75f;

        while (attackTime <= attackAnimLength * 0.4f)
        {
            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / (attackAnimLength * 0.4f));

            attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + currDegree, 0);

            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
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
                attackEffect = Instantiate(baEffectRed[playerAttackController.ComboCount]);
                break;
            case InkType.GREEN:
                attackEffect = Instantiate(baEffectGreen[playerAttackController.ComboCount]);
                break;
            case InkType.BLUE:
                attackEffect = Instantiate(baEffectBlue[playerAttackController.ComboCount]);
                break;

        }

        // 0.5f is distance offset
        attackEffect.transform.position = playerUtils.Tr.position - (0.5f * playerUtils.ModelTr.forward);
        attackEffect.transform.rotation = Quaternion.Euler(attackEffect.transform.rotation.eulerAngles.x, playerUtils.ModelTr.eulerAngles.y, 180f);

        Destroy(attackEffect, attackAnimLength * 0.4f);
    }

    private void PlayAudio()
    {
        switch (playerAttackController.ComboCount)
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

        playerUtils = baContext.playerUtils;
        playerAnim = baContext.playerAnim;
        playerState = baContext.playerState;
        playerTarget = baContext.playerTarget;
        targetMarker = baContext.targetMarker;
        basicAttackCollider = baContext.basicAttackCollider;
        playerAttackController = baContext.playerAttackController;

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
        playerAttackController.ComboCount += 1;
        if(playerAttackController.ComboCount >= 3)
        {
            playerAttackController.ComboCount = 0;
        }
    }

    public void ExcuteAnim()
    {
        playerAnim.SetAnimationTrigger("Attack");
    }
}
