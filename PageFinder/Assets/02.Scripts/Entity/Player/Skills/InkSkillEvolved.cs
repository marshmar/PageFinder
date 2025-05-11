using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InkSkillEvolved : Skill
{
    [SerializeField] private GameObject skillEffect;
    private WaitForSeconds tickThreshold;
    private float slowAmount = 0.3f;

    public override void Start()
    {
        base.Start();
        SetSkillData();
    }

    public override void ActiveSkill()
    {
        tickThreshold = new WaitForSeconds(0.5f);
    }

    private IEnumerator DropInkSequence()
    {
         for(int i = 0; i < 3; i++)
         {     
            yield return tickThreshold;
            Collider[] enemies = FindEnemiesInRange();
            // 스킬 이펙트 생성
            foreach(var enemy in enemies)
            {
                ApplyDamage(enemy, skillBasicDamage);
                ApplySlow(enemy, slowAmount);
                ApplyExtraEffectByInkType(enemy);
            }
         }
    }

    private Collider[] FindEnemiesInRange()
    {
        int enemyLayer = 1 << 6;
        return Physics.OverlapSphere(transform.position, skillRange, enemyLayer);
    }

    private void ApplyDamage(Collider enemy, float damageAmount)
    {
        Enemy enemyComp = enemy.GetComponent<Enemy>();
        if(enemyComp != null)
        {
            enemyComp.Hit(skillInkType, damageAmount);
        }
    }

    private void ApplySlow(Collider enemy, float amount)
    {
        EnemyBuff enemyBuffComp = enemy.GetComponent<EnemyBuff>();
        if(enemyBuffComp != null)
        {
            //BuffData buffData = new BuffData();
            //enemyBuffComp.AddBuff();
        }
    }


    private void ApplyExtraEffectByInkType(Collider enemy)
    {
        
    }

    private void GenerateInkMark()
    {

    }

    private void GenerateEffect()
    {

    }
}
