using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
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

    public virtual float HP {
        get {
            return currHP;
        } 
        set {
            currHP = value;
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
    public virtual void Start()
    {
        currHP = maxHP;
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
