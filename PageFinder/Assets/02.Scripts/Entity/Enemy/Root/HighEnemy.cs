using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighEnemy : EnemyAction
{
    #region Variables
    [Header("Skill")]
    protected List<float> maxSkillCoolTimes = new List<float>(); // ��ų ��Ÿ�� 
    protected List<float> currSkillCoolTimes = new List<float>(); // ���� ��ų ��Ÿ�� 
    protected List<int> skillPriority = new List<int>(); // ��ų �켱��
    protected List<bool> skillConditions = new List<bool>(); // ��ų ����
    protected int currSkillNum; // -1 : �ƹ� ��ų�� ������� �ʴ� ����   0 : ��ų0    1: ��ų1   2: ��ų2

    #endregion

    #region Init

    protected override void InitStatValue()
    {
        base.InitStatValue();

        foreach(var coolTime in maxSkillCoolTimes)
            currSkillCoolTimes.Add(coolTime);

        currSkillNum = -1; // �ƹ� ��ų�� ������� �ʴ� ����
    }

    public override void InitStat(EnemyData enemyData)
    {
        base.InitStat(enemyData);

        maxSkillCoolTimes = enemyData.skillCoolTimes;
        skillPriority = enemyData.skillPriority;

        for (int i = 0; i < skillPriority.Count; i++)
            skillConditions.Add(false);
    }

    #endregion

    #region State

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();

        switch(attackValue)
        {
            case -1:
                attackState = AttackState.BASIC;
                break;

            // ��� : ��ų�� 1��
            case 0:
                attackState = AttackState.SKILL;
                break;
        }
    }

    #endregion

    #region Cool Time

    protected override void SetAllCoolTime()
    {
        base.SetAllCoolTime();
        SetSkillCooltime();
    }

    private void SetSkillCooltime()
    {
        SetCurrSkillCoolTime();
        CheckSkillsCondition();
    }


    #endregion

    #region Skill

    protected virtual int GetTypeOfAttackToUse()
    {
        /* <�� ��� �� ��ƾ>
         *  �ϱ� : skillCoolTime.Count == 0 => ���� false
         *  ��� : ��ų 1�� => �ش� ��ų ��Ÿ�� üũ => 0�̸� true �ƴϸ� false
         *  �߰����� : ��ų 2�� => ��Ȳ�� ���� � ��ų�� ������� üũ 
         */

        // ��ų �켱���� ���� ��
        // �켱 ������ ���� ��ų ������� ��Ÿ���� ���Ҵ��� üũ : ���ڰ� ���� ���� �켱 ������ ����
        for (int priority = 0; priority < skillPriority.Count; priority++)
        {
            int skillNum = skillPriority[priority];

            // �ش� ��ų ��Ÿ�Ӱ� ��� ���� üũ
            if (currSkillCoolTimes[skillNum] <=0 && skillConditions[skillNum])
            {
                currSkillNum = skillNum;
                return skillNum;
            }
        }

        // �⺻ ����
        return -1;
    }


    private void SetCurrSkillCoolTime()
    {
        for (int i = 0; i < currSkillCoolTimes.Count; i++)
        {
            if (i == currSkillNum) // �ش� ��ų ������� ����
                continue;

            if (currSkillCoolTimes[i] < 0)
                continue;

            currSkillCoolTimes[i] -= Time.deltaTime;
        }
    }

    protected virtual void CheckSkillsCondition()
    {

    }

    #endregion

    #region Animation

    protected override void AttackAni()
    {
        switch (attackState)
        {
            case AttackState.BASIC:
                SetAniVariableValue(AttackState.BASIC);
                break;

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                break;
        }
    }

    protected virtual void SkillAni()
    {
        // �ƹ� ��ų�� ������� �ʴ� ����
        if (currSkillNum == -1)
            return;

        SetAniVariableValue(AttackState.SKILL + currSkillNum); // ��ų �ε����� ���� �ִϸ��̼� �� �����ϵ��� ��
    }

    /// <summary>
    /// Skill �ִϸ��̼��� ������ ȣ��Ǵ� �Լ� (Inspector - Events)
    /// </summary>
    public virtual void SkillEnd() // public : CircleRange�� ���� �ٸ� ��ũ��Ʈ������ ���
    {
        // ����� ��ų ���� : ����� ��ų ��������� ���� ������ false�� �ʱ�ȭ���� �ʴ´�.
        if (rank != Rank.ELITE)
            skillConditions[currSkillNum] = false;

        // ��ų ��Ÿ��
        currSkillCoolTimes[currSkillNum] = maxSkillCoolTimes[currSkillNum];

        // ���� ��ų 
        currSkillNum = -1;

        attackState = AttackState.NONE;

        //Debug.Log("��ų ��");
    }

    #endregion
}
