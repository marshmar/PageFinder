using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    /*// ���ش� �߰�
    [SerializeField]
    protected float originalMoveSpeed;
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected float maxHP;
    [SerializeField]
    protected float currHP;
    [SerializeField]
    protected float atk;
    [SerializeField]
    protected float def;

    

    //[Header("Bar")]
    //[SerializeField]
    //protected ShieldBar shieldBar;
    //[SerializeField]
    //protected SliderBar hpBar;

    protected float maxShield;
    protected float currShield;

    public virtual float HP {
        get {
            return currHP;
        } 
        set {
            currHP  = value;
            //hpBar.SetCurrValueUI(currHP);

            if (currHP <= 0)
            {
                Die();
            }
        }
    }

    public virtual float MAXHP
    {
        get
        {
            return maxHP;
        }
        set 
        {
            maxHP = value;
            //hpBar.SetMaxValueUI(maxHP);
        }
    }

    public virtual float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
        }
    }
    public float OriginalMoveSpeed { 
        get => originalMoveSpeed; set => originalMoveSpeed = value; }
    public virtual float ATK
    {
        get { return atk; }
        set
        {
            atk = value;
        }
    }

    public virtual float DEF
    {
        get { 
            return def; 
        }
        set
        {
            def = value;
        }
    }

    #region ���� ����

    public virtual float MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            // �ǵ带 ������ ���

            maxShield = value;
            //hpBar.SetMaxValueUI(maxHP + maxShield);
            
            //shieldBar.SetMaxValueUI(maxHP, currHP, maxShield);
            CurrShield = maxShield;
        }
    }

    public virtual float CurrShield
    {
        get
        {
            return currShield;
        }
        set
        {
            currShield = value;

            //shieldBar.SetCurrValueUI(currShield);

            // ���带 �� ������� ���
            if (currShield <= 0)
           currShield = 0;
        }
    }

    

    #endregion

    public virtual void Start()
    {
        currHP = maxHP;
        
        // HP Bar
        //hpBar = GetComponentInChildren<SliderBar>();
        //shieldBar = GetComponentInChildren<ShieldBar>();
        //hpBar.SetMaxValueUI(maxHP);
        //hpBar.SetCurrValueUI(currHP);

        //MaxShield = 0;
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }*/
}
