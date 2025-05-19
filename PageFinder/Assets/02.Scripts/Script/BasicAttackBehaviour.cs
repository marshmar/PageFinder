using System;
using System.Collections;
using UnityEngine;

public class BasicAttackBehaviour : MonoBehaviour, IScriptBehaviour
{
    private int ComboCount;
    private Collider target;
    private PlayerUtils playerUtils;
    private PlayerState playerState;
    private PlayerTarget playerTarget;
    private PlayerAnim playerAnim;
    private TargetObject targetMarker;
    private PlayerBasicAttackCollider basicAttackCollider;
    private ScriptData scriptData;
    private GameObject attackEffect;
    private Coroutine attackCoroutine;

    [SerializeField] private GameObject[] baEffectRed;
    [SerializeField] private GameObject[] baEffectGreen;
    [SerializeField] private GameObject[] baEffectBlue;

    public bool CanExcuteBehaviour()
    {
        return true;
    }

    public void ExcuteBehaviour()
    {
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

        // 애니메이션 재생은 플레이어 컨트롤러에서 진행

        // 공격 오브젝트 움직이기
        SweepArkAttackEachComboStep();

        // 사운드 재생
        PlayAudio();
        // 이팩트 재생
        GenerateEffect();
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


    private void SweepArkAttackEachComboStep()
    {
        switch (ComboCount)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
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
        float currAnimationLength = playerAnim.GetCurrAnimLength() * 0.75f;

        while (attackTime <= currAnimationLength * 0.4f)
        {
            if (!isAttacking)
            {
                attackObj.SetActive(false);
            }

            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / (currAnimationLength * 0.4f));

            attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + currDegree, 0);

            if (playerDashControllerScr.IsDashing || playerSkillControllerScr.IsUsingSkill)
            {
                playerMoveController.CanMove = true;
                playerMoveController.MoveTurn = true;

                if (attackEffect != null)
                {
                    Destroy(attackEffect);
                    attackEffect = null;
                }
                break;
            }

            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, playerUtils.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        attackObj.SetActive(false);
        yield break;
    }

    public void GenerateInkMark()
    {
        
    }

    public void GenerateEffect()
    {
        attackEffect = null;
        switch (scriptData.inkType)
        {
            case InkType.RED:
                attackEffect = Instantiate(baEffectRed[ComboCount], this.transform);
                break;
            case InkType.GREEN:
                attackEffect = Instantiate(baEffectGreen[ComboCount], this.transform);
                break;
            case InkType.BLUE:
                attackEffect = Instantiate(baEffectBlue[ComboCount], this.transform);
                break;

        }

        // 0.5f is distance offset
        attackEffect.transform.position = playerUtils.Tr.position - (0.5f * playerUtils.ModelTr.forward);
        attackEffect.transform.rotation = Quaternion.Euler(attackEffect.transform.rotation.eulerAngles.x, playerUtils.ModelTr.eulerAngles.y, 180f);
    }

    private void PlayAudio()
    {
        switch (ComboCount)
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

    public void SetContext()
    {

    }

}
