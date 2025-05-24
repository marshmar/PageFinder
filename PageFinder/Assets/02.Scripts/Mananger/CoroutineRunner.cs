using UnityEngine;
using System.Collections;

public class CoroutineRunner : Singleton<CoroutineRunner>
{
    public Coroutine RunCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopRunningCoroutine(Coroutine coroutine)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
