using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager<T> :Singleton<SkillManager<T>> where T : UnityEngine.Object 
{
    protected Dictionary<string, T> skillDic;
    protected T[] skillArr;

    public T this[string name]
    {
        get
        {
            Debug.Log(skillDic.Count);

            if (skillDic.ContainsKey(name))
            {
                return skillDic[name];
            }

            else
            {    
                // T가 Value Type일 경우에는 0, Reference Type일 경우에는 null 반환
                return default(T);
            }

        }
        set
        {
            if (skillDic.ContainsKey(name))
                skillDic[name] = value;
            else
                skillDic.Add(name, value);
        }
    }

    public virtual void Start()
    {
        skillDic = new Dictionary<string, T>();
        LoadSkill();
    }
    public void LoadSkill()
    {
        skillArr = Resources.LoadAll<T>("");
        for (int i = 0; i < skillArr.Length; i++)
        {
            skillDic.Add(skillArr[i].ToString(), skillArr[i]);
        }
    }
}
