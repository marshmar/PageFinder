using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 매니저
/// 스킬 데이터와 스킬 프리팹을 관리함.
/// 스킬 데이터와 스킬 프리팹은 게임 최초 시작시 실행.
/// 그 이후 Singleton으로 존재하여 계속 파괴되지 않고 진행.
/// </summary>
public class SkillManager : Singleton<SkillManager>
{

    private GameObject[] skillObjects;
    private ScriptableObject[] skillDatas;
    // 스킬 프리팹 딕셔너리 
    private Dictionary<string, GameObject> skillPrefabDic;
    // 스킬 데이터 딕셔너리
    private Dictionary<string, SkillData> skillDataDic;

    public override void Awake()
    {
        base.Awake();

        skillPrefabDic = new Dictionary<string, GameObject>();
        skillDataDic = new Dictionary<string, SkillData>();

        // 스킬 프리팹 로드
        LoadSkillPrefabs();
        // 스킬 데이터 로드
        LoadSkillDatas();
    }

    public void Start()
    {

    }

    /// <summary>
    /// 스킬 프리팹을 로드하는 함수
    /// 최초 1번만 실행
    /// </summary>
    private void LoadSkillPrefabs()
    {
        skillObjects = Resources.LoadAll<GameObject>("SkillPrefabs");
        if (skillObjects == null) return;   // 가져온 스킬 프리팹이 없으면 return
        for (int i = 0; i < skillObjects.Length; i++)
        {
            skillPrefabDic.Add(skillObjects[i].name, skillObjects[i]);
        }
    }

    /// <summary>
    /// 스킬 데이터를 로드하는 함수
    /// 최초 1번만 실행
    /// </summary>
    private void LoadSkillDatas()
    {
        skillDatas = Resources.LoadAll<ScriptableObject>("SkillDatas");
        if (skillDatas == null) return; // 가져온 스킬 데이터가 없으면 return
        SkillData skillData;
        for (int i = 0; i < skillDatas.Length; i++)
        {
            skillData = skillDatas[i] as SkillData;
            if (skillData == null) continue; // skillData로 형변환이 실패하면 continue
            skillDataDic.Add(skillData.name, skillData);
        }
    }

    /// <summary>
    /// 지정된 이름의 스킬 프리팹을 받아오는 함수
    /// </summary>
    /// <param name="skillPrefabName">가져올 스킬 프리팹 이름</param>
    /// <returns>지정된 이름의 스킬 프리팹이 딕셔너리에 존재할 경우 해당 스킬 프리팹 반환, 그렇지 않을 경우 null반환</returns>
    public GameObject GetSkillPrefab(string skillPrefabName)
    {
        if (skillPrefabDic.ContainsKey(skillPrefabName))
        {
            return skillPrefabDic[skillPrefabName];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 지정된 이름의 스킬 데이터를 받아오는 함수
    /// </summary>
    /// <param name="skillDataName">가져올 스킬 데이터 이름</param>
    /// <returns>지정된 이름의 스킬 데이터가 딕셔너리에 존재할 경우 해당 스킬 데이터 반환, 그렇지 않을 경우 null반환</returns>
    public SkillData GetSkillData(string skillDataName)
    {
        if (skillDataDic.ContainsKey(skillDataName))
        {
            return skillDataDic[skillDataName];
        }
        else
        {
            return null;
        }
    }
}
