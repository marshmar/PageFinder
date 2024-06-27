using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected float moveSpeed;
    protected float maxHP;
    protected float currHP;
    protected float atk;
    protected float def;
     
    public virtual float HP {
        get {
            return currHP; 
        } 
        set {
            currHP = value;
            if(currHP <= 0)
            {
                Die();
            }
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
            atk += value;
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
