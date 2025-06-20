using System;
using System.Collections;
using UnityEngine;

public class BasicAttackContext : ScriptContext
{
    public Player Player;
    public GameObject[] BaEffectRed;
    public GameObject[] BaEffectGreen;
    public GameObject[] BaEffectBlue;
}


public class BasicAttackBehaviour : MonoBehaviour, IScriptBehaviour
{
    private float _attackAnimLength;
    private const float _attackProgressThreshold = 0.8f;
    private Tuple<float, float>[] _attackDegrees = new Tuple<float, float>[3] {
        new Tuple<float, float>(-45.0f, 90.0f),  
        new Tuple<float, float>(45.0f, -90.0f),  
        new Tuple<float, float>(-70.0f, 140.0f),
    };

    // hashing
    private Collider _target;
    private Player _player;
    private NewScriptData _scriptData;
    private GameObject _attackEffect;
    private Coroutine _attackCoroutine;
    private short[] _baSfxIndexs = new short[3] { Sound.attack1Sfx, Sound.attack2Sfx, Sound.attack3Sfx };

    // attack effect
    private GameObject[] _baEffectRed;
    private GameObject[] _baEffectGreen;
    private GameObject[] _baEffectBlue;

    public event Action AfterEffect;

    public bool CanExcuteBehaviour()
    {
        if (_player.AttackController.IsAttacking && !_player.Anim.HasAttackAnimPassedTime(_attackProgressThreshold))
            return false;

        if (_player.AttackController.IsNextAttackBuffered)
            return false;

        if (_player.DashController.IsDashing || _player.SkillController.IsUsingSkill)
            return false;

        if (!_player.AttackController.IsAbleAttack)
            return false;

        if(_player.AttackController.IsAttacking && _player.Anim.HasAttackAnimPassedTime(_attackProgressThreshold))
        {
            _player.AttackController.IsNextAttackBuffered = true;
        }

        return true;
    }

    public void ExcuteBehaviour()
    {
        if (_player.IsNull()) return;
        _player.AttackController.IsAttacking = true;

        // 에너미 찾기
        FindTarget();

        // 기본공격 범위 보여주기
        ShowAttackRange();

        // 에너미 존재시 타켓 마커 활성화 및 플레이어 회전
        if(_target != null)
        {
            ShowTargetMarker();
            RotateToTarget();
        }

        SetBasicAttackColliderInkType();

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

    private void SetBasicAttackColliderInkType()
    {
        _player.BasicAttackCollider.BaInkType = _scriptData.inkType;
    }

    public void StopBehaviour()
    {
        if(_attackCoroutine != null)
        {
            CoroutineRunner.Instance.StopRunningCoroutine(_attackCoroutine);
            _attackCoroutine = null;
            _player.BasicAttackCollider.gameObject.SetActive(false);
        }

        if(_attackEffect != null)
        {
            Destroy(_attackEffect);
            _attackEffect = null;
        }
    }


    private void FindTarget()
    {
        if (_player.Utils.IsNull()) return;
        if (_player.State.IsNull()) return;

        int targetLayer = LayerMask.GetMask("ENEMY") + LayerMask.GetMask("INTERACTIVEOBJECT");

#if UNITY_STANDALONE
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, targetLayer))
        {
            Collider primaryTarget = hit.collider;
            if (Vector3.Distance(_player.Utils.Tr.position, primaryTarget.transform.position) <= _player.State.CurAttackRange.Value)
            {
                _target = primaryTarget;
                return;
            }
        }
#endif 
        if(_target != null)
        {
            if(Vector3.Distance(_target.transform.position, _player.Utils.Tr.position) <= _player.State.CurAttackRange.Value)
            {
                return;
            }
        }

        _target = Utils.FindMinDistanceObject(_player.Utils.Tr.position, _player.State.CurAttackRange.Value, targetLayer);
    }

    private void ShowAttackRange()
    {
        if (_player.TargetingVisualizer.IsNull()) return;

        float showingTime = 0.1f;
        _player.TargetingVisualizer.ShowMaximumRange(_player.State.CurAttackRange.Value, showingTime);
    }


    private void ShowTargetMarker()
    {
        if (_player.TargetMarker.IsNull()) return;

        _player.TargetMarker.IsActive = true;
        _player.TargetMarker.TargetTransform = _target.transform;
    }


    private void RotateToTarget()
    {
        if (_player.Utils.IsNull()) return; 

        Vector3 dirToTarget = _player.Utils.CalculateDirectionFromPlayer(_target);
        _player.Utils.TurnToDirection(dirToTarget);
    }

    private void SweepArkAttackEachComboStep()
    {
        float attackDegreeStart = _attackDegrees[_player.AttackController.ComboCount].Item1;
        float attackDegreeEnd   = _attackDegrees[_player.AttackController.ComboCount].Item2;
        _attackCoroutine = CoroutineRunner.Instance.RunCoroutine(SweepArkAttack(attackDegreeStart, attackDegreeEnd));
    }

    private IEnumerator SweepArkAttack(float startDegree, float degreeAmount)
    {
        if (_player.BasicAttackCollider.IsNull()) yield break;
        if (_player.Anim.IsNull()) yield break;


        GameObject attackObj = _player.BasicAttackCollider.gameObject;

        attackObj.SetActive(true);
        attackObj.transform.localPosition = Vector3.zero;

        float attackTime = 0;
        float currDegree = startDegree;
        float targetDegree = startDegree + degreeAmount;

        attackObj.transform.rotation = Quaternion.Euler(0, _player.Utils.ModelTr.rotation.eulerAngles.y + startDegree, 0);

        float animSpeedMutilpier = 0.75f;
        _attackAnimLength = _player.Anim.GetAdjustedAnimDuration() * animSpeedMutilpier;

        animSpeedMutilpier = 0.4f;
        while (attackTime <= _attackAnimLength * animSpeedMutilpier)
        {
            attackTime += Time.deltaTime;
            currDegree = Mathf.Lerp(startDegree, targetDegree, attackTime / (_attackAnimLength * animSpeedMutilpier));

            attackObj.transform.rotation = Quaternion.Euler(0, _player.Utils.ModelTr.rotation.eulerAngles.y + currDegree, 0);

            yield return null;
        }

        attackObj.transform.rotation = Quaternion.Euler(0, _player.Utils.ModelTr.rotation.eulerAngles.y + targetDegree, 0);
        attackObj.SetActive(false);

        _attackCoroutine = null;
        yield break;
    }

    public void GenerateInkMark(Vector3 position)
    {
        
    }

    public void GenerateEffect()
    {
        _attackEffect = null;
        switch (_scriptData.inkType)
        {
            case InkType.RED:
                _attackEffect = Instantiate(_baEffectRed[_player.AttackController.ComboCount]);
                break;
            case InkType.GREEN:
                _attackEffect = Instantiate(_baEffectGreen[_player.AttackController.ComboCount]);
                break;
            case InkType.BLUE:
                _attackEffect = Instantiate(_baEffectBlue[_player.AttackController.ComboCount]);
                break;

        }

        const float effectDistanceOffeset = 0.5f;
        _attackEffect.transform.position = _player.Utils.Tr.position - (effectDistanceOffeset * _player.Utils.ModelTr.forward);
        _attackEffect.transform.rotation = Quaternion.Euler(_attackEffect.transform.rotation.eulerAngles.x, _player.Utils.ModelTr.eulerAngles.y, 180f);

        const float animSpeedMultiplier = 0.7f;
        Destroy(_attackEffect, _attackAnimLength * animSpeedMultiplier);
    }

    private void PlayAudio()
    {
        AudioManager.Instance.Play(_baSfxIndexs[_player.AttackController.ComboCount], AudioClipType.BaSfx);
    }

    public void SetContext(ScriptContext context)
    {
        BasicAttackContext baContext = context as BasicAttackContext;
        if(baContext == null)
        {
            Debug.LogError("Failed to Convert BasicAttackContext");
            return;
        }
        _player = baContext.Player;
        _baEffectRed = baContext.BaEffectRed;
        _baEffectGreen = baContext.BaEffectGreen;
        _baEffectBlue = baContext.BaEffectBlue;
    }

    public void SetScriptData(NewScriptData scriptData)
    {
        this._scriptData = scriptData;
    }

    private void IncreaseCombo()
    {
        _player.AttackController.ComboCount += 1;
        if(_player.AttackController.ComboCount >= 3)
        {
            _player.AttackController.ComboCount = 0;
        }
    }

    public void ExcuteAnim()
    {
        _player.Anim.SetAnimationTrigger("Attack");
    }

    public void SetExtraDamageMultiplier(float amount)
    {
        _player.BasicAttackCollider.ExtraDamageMultiplier = amount;
    }
}
