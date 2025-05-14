using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InkSkillEvolved : Skill
{
    [SerializeField] private GameObject[] skillEffects;
    private WaitForSeconds tickThreshold;
    private float slowAmount = -0.3f;

    public override void Start()
    {
        base.Start();
        SetSkillData();
        tickThreshold = new WaitForSeconds(0.3f);
        //ActiveSkill();
    }

    private void Update()
    {

    }

    public override void ActiveSkill()
    {
        GenerateEffect();
        StartCoroutine(DropInkSequence());
    }

    private IEnumerator DropInkSequence()
    {
        yield return new WaitForSeconds(0.4f);
        Collider[] enemies;
        for (int i = 0; i < 3; i++)
        {     
            yield return tickThreshold;
            enemies = FindEnemiesInRange();
            foreach(var enemy in enemies)
            {
                ApplyDamage(enemy, skillBasicDamage, Enemy.DebuffState.NONE);
                ApplySlow(enemy, slowAmount);
                ApplyExtraEffectByInkType(enemy);
            }
        }

        yield return new WaitForSeconds(0.6f);

        enemies = FindEnemiesInRange();

        foreach (var enemy in enemies)
        {
            ApplyDamage(enemy, skillBasicDamage * 1.5f, Enemy.DebuffState.STUN);
            ApplySlow(enemy, slowAmount);
            ApplyExtraEffectByInkType(enemy);
        }

        yield return new WaitForSeconds(0.1f);
        GenerateInkMark();
    }

    private Collider[] FindEnemiesInRange()
    {
        int enemyLayer = 1 << 6;
        return Physics.OverlapSphere(transform.position, skillRange * 0.5f, enemyLayer);
    }

    private void ApplyDamage(Collider enemy, float damageAmount, Enemy.DebuffState debuffState)
    {
        Enemy enemyComp = enemy.GetComponent<Enemy>();
        if(enemyComp != null)
        {
            enemyComp.Hit(skillInkType, damageAmount, debuffState, 2.0f);
        }
    }

    private void ApplySlow(Collider enemy, float slowAmount)
    {
        EnemyBuff enemyBuffComp = enemy.GetComponent<EnemyBuff>();
        if(enemyBuffComp != null)
        {
            enemyBuffComp.AddBuff(new BuffData(BuffType.BuffType_Temporary, 1000, slowAmount, 2f, targets: new List<Component>() { enemy.transform.GetComponent<Enemy>() }));
        }
    }


    private void ApplyExtraEffectByInkType(Collider enemy)
    {
        // https://docs.google.com/document/d/1y1SKt0nX8KDx6ITJoY5QB0G-MSX9Q5O9W_TOh_VwNjM/edit?tab=t.0#heading=h.xmejaqboszb5
        // 적중시 추가효과 적용 필요
    }

    private void GenerateInkMark()
    {
        InkMark inkMark = InkMarkPooler.Instance.Pool.Get();

        inkMark.SetInkMarkData(InkMarkType.INKSKILLEVOLVED, skillInkType, false);
        inkMark.IsAbleFusion = false;

        Transform inkObjTransform = inkMark.transform;
        inkObjTransform.SetPositionAndRotation(transform.position + new Vector3(0f, 0.1f, 0f), Quaternion.Euler(90, 0f, 0f));
    }

    private void GenerateEffect()
    {
        GameObject skillEffect = null;
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        switch (skillInkType)
        {
            case InkType.RED:
                skillEffect = Instantiate(skillEffects[0], transform.position, rotation);
                break;
            case InkType.GREEN:
                skillEffect = Instantiate(skillEffects[1], transform.position, rotation);
                break;
            case InkType.BLUE:
                skillEffect = Instantiate(skillEffects[2], transform.position, rotation);
                break;
        }

        skillEffect.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        Destroy(skillEffect, 2.0f);
    }
}
