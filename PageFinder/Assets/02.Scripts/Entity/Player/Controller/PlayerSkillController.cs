using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : Player
{
    private SkillManager<GameObject> skillObjectManager;
    private SkillManager<SkillData> skillDataManager;

/*    private GameObject[] skillObjects;
    private ScriptableObject[] skillDatas;*/
    // 스킬 프리팹 딕셔너리 
    //private Dictionary<string, GameObject> skillPrefabs;
    // 스킬 데이터 딕셔너리
    //private Dictionary<string, SkillData> skillDataDics;


    private GameObject skillObject;
    private SkillData skillData;

    private Vector3 spawnVector;

    // 공격할 적 객체
    Collider attackEnemy;

    private new void Awake()
    {
        skillObjectManager = SkillObjectManager.Instance;
        skillDataManager = SkillDataManager.Instance;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    private void LoadSkillPrefabs()
    {
        skillPrefabs = new Dictionary<string, GameObject>();
        skillObjects = Resources.LoadAll<GameObject>("Skills");
        for (int i = 0; i < skillObjects.Length; i++)
        {
            skillPrefabs.Add(skillObjects[i].name, skillObjects[i]);
        }
    }*/

/*    private void LoadSkillDatas()
    {
        skillDataDics = new Dictionary<string, SkillData>();
        skillDatas = Resources.LoadAll<ScriptableObject>("SkillDatas");
        for (int i = 0; i < skillDatas.Length; i++)
        {
            skillData = skillDatas[i] as SkillData;
            skillDataDics.Add(skillData.name, skillData);
        }
    }*/

    /// <summary>
    /// 가장 가까운 적에게 스킬을 소환하는 함수
    /// </summary>
    /// <param name="skillName"></param>
    public void InstantiateSkill(string skillName)
    {
        Debug.Log(skillName);
        skillObject = skillObjectManager[skillName];
        if (skillObject == null)
        {
            Debug.LogError("소환할 스킬 오브젝트가 없습니다.");
            return;
        }

        skillData = skillDataManager[skillName];
        if (skillData == null)
        {
            Debug.LogError("스킬 데이터 존재 x");
            return;
        }

        switch (skillData.skillType)
        {
            case SkillTypes.PAINT:
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, skillData.skillDist, 1 << 6);

                if (attackEnemy == null)
                {
                    Debug.Log("공격할 적 객체가 없습니다.");
                    return;
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
        Instantiate(skillObject, spawnVector, Quaternion.identity);
    }

    // 지정한 위치에 스킬 소환하는 함수
    public void InstantiateSkill(string skillName, Vector3 pos)
    {
        skillObject = skillObjectManager[skillName];
        if (skillObject == null) 
        { 
            Debug.LogError("소환할 스킬 오브젝트가 없습니다.");
            return;
        }
        skillData = skillDataManager[skillName];

        if (skillData == null)
        {
            Debug.LogError("스킬 데이터 존재 x");
            return;
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
    }

/*    public GameObject GetSkillPrefabs(string skillName)
    {
        if (skillPrefabs.ContainsKey(skillName))
        {
            return skillPrefabs[skillName];
        }
        else
        {
            Debug.LogError("스킬 오브젝트 없음");
        }
        return null;
    }

    public SkillData GetSkillData(string skillName)
    {
        if (skillDataDics.ContainsKey(skillName))
        {
            return skillDataDics[skillName];
        }
        return null;
    }*/

}
