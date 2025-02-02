using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VFX.UI;
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

    /// <summary>
    /// 내구도에 가한 잉크 정보
    /// </summary>
    struct InkData
    {
        public InkType type;
        public float damage;
        public int order; // 0 : 가장 최근에 사용      max : 가장 오래전에 사용
    }

    private float durability;
    private List<InkData> inkDatas = new List<InkData>();

    private float explosionRange;
    private float explosionDamage;

    private float inkSize;
    private Vector3 inkPos;
    private float inkDuration;

    private void Start()
    {
        Init();
    }

    // 지금은 아래와 같이 값을 임시로 할당하지만 나중에는 CSV를 통해 값이 들어갈 수 있도록 변경 필요
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
    /// 내구도를 설정한다.
    /// </summary>
    /// <param name="inkType">사용한 잉크 속성</param>
    /// <param name="damage">가할 데미지</param>
    public void SetDurability(InkType inkType, float damage)
    {
        durability -= damage;

        // InkData
        SetInkData(inkType, damage);

        if (durability > 0)
            return;

        // 내구도가 0보다 작을 경우
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
            // 이미 내구도에 데미지를 가한 속성일 경우
            if (inkDatas[i].type == inkType)
            {
                var tmpInkData = inkDatas[i];
                tmpInkData.damage += damage;
                inkDatas[i] = tmpInkData;
                SetInkOrder(inkDatas[i].type);
                return;
            }
        }

        // 내구도에 데미지를 처음 가하는 속성일 경우
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

            // 가장 최근에 사용한 잉크일 경우
            if (inkDatas[i].type == inkType)
                tmpInkData.order = 0;
            else
            {
                // 가장 오래전에 사용한 속성이 아닐 경우
                if (tmpInkData.order != inkDatas.Count - 1)
                    tmpInkData.order++;
            }
            inkDatas[i] = tmpInkData;
        }
    }

    /// <summary>
    /// 기장 큰 기여를 한 잉크 속성을 얻는다. 
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

            // 데미지 값이 같을 경우 가장 최근에 사용한 속성으로 결정하기 위함
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
                    Debug.Log($"폭발 피해 - {collider.name} : {entityScr.HP}");
                    break;

                default:
                    continue;
            }
        }
    }

    private void ApplyExplosiveEffectToEnemy(GameObject enemy, InkType inkType)
    {
        Enemy enemyScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(enemy, "Enemy");

        switch (inkType)
        {
            case InkType.RED:
                enemyScr.SetDebuff(Enemy.DebuffState.STUN, 1, Vector3.zero);
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

        inkMark.transform.position = new Vector3(transform.position.x, transform.position.y -0.4f, transform.position.z); // 나중에 2층 지형 생기면 1.1이 아니라 능동적으로 변할 수 있도록 바꾸어야 함
        inkMark.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
