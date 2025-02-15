using UnityEngine;

public abstract class BuffFactory
{
    public abstract BuffCommand CreateBuffCommand(ref BuffData buffData);
}
