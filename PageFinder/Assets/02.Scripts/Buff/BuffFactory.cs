using UnityEngine;

public abstract class BuffFactory
{
    public abstract BuffCommand CreateBuffCommand(in BuffData buffData);
}
