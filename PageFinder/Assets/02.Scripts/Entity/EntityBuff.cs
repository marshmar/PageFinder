using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBuff : MonoBehaviour
{
    protected BuffCommandInvoker buffCommandInvoker;

    public abstract void AddBuff(int buffID);

    public abstract void RemoveBuff(int buffID);

    public abstract void ChangeBuffLevel(int buffID, int level);
}
