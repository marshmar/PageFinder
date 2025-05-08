using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 그냥 스탯 데이터 클래스 하나 만들어서 참조로 가지고 있는게 나을것 같다.
/// </summary>

/// <summary>
/// 공통 프로퍼티 지정 필요
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
