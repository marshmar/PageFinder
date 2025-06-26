using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBox : MonoBehaviour
{
    /*
 *<������ �ڽ�>
   - ������ : �⺻ 100, 0�� �����ϸ� ���� Ȱ��ȭ
    - �������� ���ҵ� ������ (� �Ӽ�������, ���������� ���� ��)������ �ؾ� ��

  <����>
    - ������
    - ��ũ �Ӽ�, �������� ���ҽ�Ų ��, ����

    - ���� ����
    - ���� ����

    - ��ũ ũ��
    - ��ũ ���� ��ġ
    - ��ũ ���� �ð�


    - �����۹ڽ��� �ᱹ CSV�� ���� ������ ������ �������� ��.
    - �����߿� �����Ǵ� ���� �ƴ϶� �ʿ� �̹� �����ϴ� ���̱� ������ start()���� CSV�� ���� �� ���ø� ���ָ� �� �� 
 */

    #region Variables
    /// <summary>
    /// �������� ���� ��ũ ����
    /// </summary>
    private struct InkData
    {
        public InkType Type;
        public float Damage;
        public int Order; // 0 : ���� �ֱٿ� ���      max : ���� �������� ���
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

        // �������� 0���� ���� ���
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
            // �̹� �������� �������� ���� �Ӽ��� ���
            if (_inkDatas[i].Type == inkType)
            {
                var tmpInkData = _inkDatas[i];
                tmpInkData.Damage += damageAmount;
                _inkDatas[i] = tmpInkData;
                SetInkOrder(_inkDatas[i].Type);
                return;
            }
        }

        // �������� �������� ó�� ���ϴ� �Ӽ��� ���
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

            // ���� �ֱٿ� ����� ��ũ�� ���
            if (_inkDatas[i].Type == inkType)
                tmpInkData.Order = 0;
            else
            {
                // ���� �������� ����� �Ӽ��� �ƴ� ���
                if (tmpInkData.Order != _inkDatas.Count - 1)
                    tmpInkData.Order++;
            }
            _inkDatas[i] = tmpInkData;
        }
    }

    /// <summary>
    /// ���� ū �⿩�� �� ��ũ �Ӽ��� ��´�. 
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

            // ������ ���� ���� ��� ���� �ֱٿ� ����� �Ӽ����� �����ϱ� ����
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
                    //Debug.Log($"���� ���� - {collider.name} : {entityScr.HP}");
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

        inkMark.transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z); // ���߿� 2�� ���� ����� 1.1�� �ƴ϶� �ɵ������� ���� �� �ֵ��� �ٲپ�� ��
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
