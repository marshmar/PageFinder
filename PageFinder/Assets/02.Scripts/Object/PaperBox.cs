using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBox : MonoBehaviour
{
    /*
 *<페이퍼 박스>
   - 내구도 : 기본 100, 0에 도달하면 반응 활성화
    - 내구도가 감소될 때마다 (어떤 속성값인지, 마지막으로 사용된 것)저장을 해야 함

  <변수>
    - 내구도
    - 잉크 속성, 내구도를 감소시킨 양, 순서

    - 폭발 범위
    - 폭발 피해

    - 잉크 크기
    - 잉크 생성 위치
    - 잉크 지속 시간


    - 페이퍼박스도 결국 CSV를 통해 변수의 값들이 정해져야 함.
    - 게임중에 생성되는 것이 아니라 맵에 이미 존재하는 것이기 때문에 start()에서 CSV를 통해 값 세팅만 해주면 될 듯 
 */

    #region Variables
    /// <summary>
    /// 내구도에 가한 잉크 정보
    /// </summary>
    private struct InkData
    {
        public InkType Type;
        public float Damage;
        public int Order; // 0 : 가장 최근에 사용      max : 가장 오래전에 사용
    }

    private float _durability;
    private List<InkData> _inkDatas = new List<InkData>();

    private float _explosionRange;
    private float _explosionDamage;

    private float _inkSize;
    private Vector3 _inkPos;
    private float _inkDuration;

    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        Initialize();
    }
    #endregion

    #region Initialization

    private void Initialize()
    {
        _durability = 100f;
        _inkDatas.Clear();

        _explosionRange = 2f;
        _explosionDamage = 100f;

        _inkSize = 1.5f;
        _inkPos = Vector3.zero;
        _inkDuration = 7f;
    }
    #endregion

    #region Actions

    public void SetDurability(InkType inkType, float damageAmount)
    {
        _durability -= damageAmount;

        // InkData
        SetInkData(inkType, damageAmount);

        if (_durability > 0)
            return;

        // 내구도가 0보다 작을 경우
        Debug.Log("Paper Box Explosion");

        InkType determindedInkType = GetInkDataThatContributedTheMost();

        Explosion(determindedInkType);
        GenerateInkMark(determindedInkType);
        //CreateEffect();
        Destroy(gameObject);
    }

    private void SetInkData(InkType inkType, float damageAmount)
    {
        for (int i = 0; i < _inkDatas.Count; i++)
        {
            // 이미 내구도에 데미지를 가한 속성일 경우
            if (_inkDatas[i].Type == inkType)
            {
                var tmpInkData = _inkDatas[i];
                tmpInkData.Damage += damageAmount;
                _inkDatas[i] = tmpInkData;
                SetInkOrder(_inkDatas[i].Type);
                return;
            }
        }

        // 내구도에 데미지를 처음 가하는 속성일 경우
        InkData inkData;
        inkData.Type = inkType;
        inkData.Damage = damageAmount;
        inkData.Order = 0;
        _inkDatas.Add(inkData);
        SetInkOrder(inkData.Type);
    }

    void SetInkOrder(InkType inkType)
    {
        for (int i = 0; i < _inkDatas.Count; i++)
        {
            var tmpInkData = _inkDatas[i];

            // 가장 최근에 사용한 잉크일 경우
            if (_inkDatas[i].Type == inkType)
                tmpInkData.Order = 0;
            else
            {
                // 가장 오래전에 사용한 속성이 아닐 경우
                if (tmpInkData.Order != _inkDatas.Count - 1)
                    tmpInkData.Order++;
            }
            _inkDatas[i] = tmpInkData;
        }
    }

    /// <summary>
    /// 기장 큰 기여를 한 잉크 속성을 얻는다. 
    /// </summary>
    /// <returns></returns>
    InkType GetInkDataThatContributedTheMost()
    {
        InkData inkData = _inkDatas[0];
        float maxDamage = inkData.Damage;

        for (int i = 1; i < _inkDatas.Count; i++)
        {
            if (_inkDatas[i].Damage < maxDamage)
                continue;

            // 데미지 값이 같을 경우 가장 최근에 사용한 속성으로 결정하기 위함
            if (_inkDatas[i].Damage == maxDamage)
            {
                if (_inkDatas[i].Order >= inkData.Order)
                    continue;
            }
            else
                maxDamage = _inkDatas[i].Damage;

            inkData = _inkDatas[i];
        }

        return inkData.Type;
    }

    private void Explosion(InkType inkType)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange);

        foreach (Collider collider in colliders)
        {
            string tag = collider.tag;
            switch (tag)
            {
                case "PLAYER":
                    break;

                case "ENEMY":
                    ApplyExplosiveEffectToEnemy(collider.gameObject, inkType);
                    IEntityState entityScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(collider.gameObject, "Enemy") as IEntityState;
                    if (entityScr != null)
                        entityScr.CurHp -= _explosionDamage;
                    //Debug.Log($"폭발 피해 - {collider.name} : {entityScr.HP}");
                    break;

                default:
                    continue;
            }
        }
    }

    private void ApplyExplosiveEffectToEnemy(GameObject enemy, InkType inkType)
    {
        EnemyAction enemyScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy, "Enemy");

        switch (inkType)
        {
            case InkType.Red:
                enemyScr.SetDebuff(Enemy.DebuffState.STUN, 1);
                break;

            case InkType.Blue:
                enemyScr.ChangeMoveSpeed(2, 70);
                break;

            case InkType.Green:
                enemyScr.ChangeAttackSpeed(3, 70);
                break;

            default:
                break;
        }
    }

    private void GenerateInkMark(InkType inkType)
    {
        InkMark inkMark = InkMarkPooler.Instance.Pool.Get();
        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(InkMarkType.INTERACTIVEOBJECT, inkMark.transform, ref _inkDuration);

        inkMark.SetInkMarkData(InkMarkType.INTERACTIVEOBJECT, inkType);

        inkMark.transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z); // 나중에 2층 지형 생기면 1.1이 아니라 능동적으로 변할 수 있도록 바꾸어야 함
        inkMark.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion
}
