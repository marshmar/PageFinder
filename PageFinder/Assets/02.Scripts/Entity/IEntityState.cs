using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �׳� ���� ������ Ŭ���� �ϳ� ���� ������ ������ �ִ°� ������ ����.
/// </summary>

/// <summary>
/// ���� ������Ƽ ���� �ʿ�
/// </summary>
public interface IEntityState
{
    public Stat MaxHp { get;}
    public float CurHp { get; set; }
    public Stat CurAtk { get;  }
    public Stat CurMoveSpeed { get;  }
    public Stat CurAttackSpeed { get;  }

    public Stat DmgResist { get;  }

    public Stat DmgBonus { get;  }

    public Stat CurAttackRange { get; }
}
