using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : Player
{
    private SkillManager skillManager;

    private GameObject skillObject;
    private SkillData skillData;
    // 스킬 소환 벡터
    private Vector3 spawnVector;

    // 공격할 적 객체
    Collider attackEnemy;

    private new void Awake()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        skillManager = SkillManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    /// <summary>
    /// 가장 가까운 적에게 스킬을 소환하는 함수
    /// </summary>
    /// <param name="skillName">소환할 스킬</param>
    /// <return>스킬 소환 성공 여부</return>
    public bool InstantiateSkill(string skillName)
    {
        skillObject = skillManager.GetSkillPrefab(skillName);
        if (skillObject == null)
        {
            Debug.LogError("소환할 스킬 오브젝트가 없습니다.");
            return false;
        }

        skillData = skillManager.GetSkillData(skillName);
        if (skillData == null)
        {
            Debug.LogError("스킬 데이터 존재 x");
            return false; 
        }
        
        switch (skillData.skillType)
        {
            case SkillTypes.PAINT:
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, skillData.skillDist, 1 << 6);

                if (attackEnemy == null)
                {
                    Debug.Log("공격할 적 객체가 없습니다.");
                    return false;
                }
                spawnVector = new Vector3(attackEnemy.transform.position.x, tr.position.y + 0.1f, attackEnemy.transform.position.z);
                TurnToDirection(spawnVector);
                anim.SetTrigger("SpawnSkill");
                break;
            case SkillTypes.STROKE:
                anim.SetTrigger("TurningSkill");
                break;
            default:
                spawnVector = new Vector3(tr.position.x, tr.position.y + 0.1f, tr.position.z);
                break;
        }
        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayAudioClip("SkillAttack");
        Instantiate(skillObject, spawnVector, Quaternion.identity);
        return true;
    }

    // 지정한 위치에 스킬 소환하는 함수
    public bool InstantiateSkill(string skillName, Vector3 pos)
    {
        skillObject = skillManager.GetSkillPrefab(skillName);
        if (skillObject == null) 
        { 
            Debug.LogError("소환할 스킬 오브젝트가 없습니다.");
            return false;
        }
        skillData = skillManager.GetSkillData(skillName);

        if (skillData == null)
        {
            Debug.LogError("스킬 데이터 존재 x");
            return false;
        }

        switch (skillData.skillType)
        {
            case SkillTypes.PAINT:
                TurnToDirection(pos);
                anim.SetTrigger("SpawnSkill");
                break;
            case SkillTypes.STROKE:
                anim.SetTrigger("TurningSkill");
                Debug.Log("STROKE 스킬 애니메이션 재생");
                break;
            default:
                break;
        }

        Debug.Log("스킬 소환");
        Instantiate(skillObject, targetObjectTr.position, Quaternion.identity);
        return true;
    }
}
