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

    /// <summary>
    /// �������� ���� ��ũ ����
    /// </summary>
    struct InkData
    {
        public InkType type;
        public float damage;
        public int order; // 0 : ���� �ֱٿ� ���      max : ���� �������� ���
    }

    private float durability;
    private List<InkData> inkDatas = new List<InkData>();

    private float explosionRange;
    private float explosionDamage;

    private float inkSize;
    private Vector3 inkPos;
    private float inkDuration;


    private void OnEnable()
    {
        Init();
    }


    // ������ �Ʒ��� ���� ���� �ӽ÷� �Ҵ������� ���߿��� CSV�� ���� ���� �� �� �ֵ��� ���� �ʿ�
    private void Init()
    {
        durability = 100;
        inkDatas.Clear();

        explosionRange = 2;
        explosionDamage = 100;

        inkSize = 1.5f;
        inkPos = Vector3.zero;
        inkDuration = 7;
    }

    /// <summary>
    /// �������� �����Ѵ�.
    /// </summary>
    /// <param name="inkType">����� ��ũ �Ӽ�</param>
    /// <param name="damage">���� ������</param>
    public void SetDurability(InkType inkType, float damage)
    {
        durability -= damage;

        // InkData
        SetInkData(inkType, damage);

        if (durability > 0)
            return;

        // �������� 0���� ���� ���
        Debug.Log("Paper Box Explosion");

        InkType determindedInkType = GetInkDataThatContributedTheMost();
   
        Explosion(determindedInkType);
        GenerateInkMark(determindedInkType);
        //CreateEffect();
        Destroy(gameObject);
    }

    private void SetInkData(InkType inkType, float damage)
    {
        for (int i = 0; i < inkDatas.Count; i++)
        {
            // �̹� �������� �������� ���� �Ӽ��� ���
            if (inkDatas[i].type == inkType)
            {
                var tmpInkData = inkDatas[i];
                tmpInkData.damage += damage;
                inkDatas[i] = tmpInkData;
                SetInkOrder(inkDatas[i].type);
                return;
            }
        }

        // �������� �������� ó�� ���ϴ� �Ӽ��� ���
        InkData inkData;
        inkData.type = inkType;
        inkData.damage = damage;
        inkData.order = 0;
        inkDatas.Add(inkData);
        SetInkOrder(inkData.type);
    }

    void SetInkOrder(InkType inkType)
    {
        for (int i = 0; i < inkDatas.Count; i++)
        {
            var tmpInkData = inkDatas[i];

            // ���� �ֱٿ� ����� ��ũ�� ���
            if (inkDatas[i].type == inkType)
                tmpInkData.order = 0;
            else
            {
                // ���� �������� ����� �Ӽ��� �ƴ� ���
                if (tmpInkData.order != inkDatas.Count - 1)
                    tmpInkData.order++;
            }
            inkDatas[i] = tmpInkData;
        }
    }

    /// <summary>
    /// ���� ū �⿩�� �� ��ũ �Ӽ��� ��´�. 
    /// </summary>
    /// <returns></returns>
    InkType GetInkDataThatContributedTheMost()
    {
        InkData inkData = inkDatas[0];
        float maxDamage = inkData.damage;

        for (int i = 1; i < inkDatas.Count; i++)
        {
            if (inkDatas[i].damage < maxDamage)
                continue;

            // ������ ���� ���� ��� ���� �ֱٿ� ����� �Ӽ����� �����ϱ� ����
            if (inkDatas[i].damage == maxDamage)
            {
                if (inkDatas[i].order >= inkData.order)
                    continue;
            }
            else
                maxDamage = inkDatas[i].damage;

            inkData = inkDatas[i];
        }

        return inkData.type;
    }

    private void Explosion(InkType inkType)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (Collider collider in colliders)
        {
            string tag = collider.tag;
            switch (tag)
            {
                case "PLAYER":
                    break;

                case "ENEMY":
                    ApplyExplosiveEffectToEnemy(collider.gameObject, inkType);
                    Entity entityScr = DebugUtils.GetComponentWithErrorLogging<Entity>(collider.gameObject, "Entity");
                    entityScr.HP -= explosionDamage;
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
            case InkType.RED:
                enemyScr.SetDebuff(Enemy.DebuffState.STUN, 1);
                break;

            case InkType.BLUE:
                enemyScr.ChangeMoveSpeed(2, 70);
                break;

            case InkType.GREEN:
                enemyScr.ChangeAttackSpeed(3, 70);
                break;

            default:
                break;
        }
    }

    private void GenerateInkMark(InkType inkType)
    {
        InkMark inkMark = InkMarkPooler.Instance.Pool.Get();
        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(InkMarkType.INTERACTIVEOBJECT, inkMark.transform, ref inkDuration);

        inkMark.SetInkMarkData(InkMarkType.INTERACTIVEOBJECT, inkType);

        inkMark.transform.position = new Vector3(transform.position.x, transform.position.y -0.4f, transform.position.z); // ���߿� 2�� ���� ����� 1.1�� �ƴ϶� �ɵ������� ���� �� �ֵ��� �ٲپ�� ��
        inkMark.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
