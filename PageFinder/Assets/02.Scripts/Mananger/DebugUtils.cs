using UnityEngine;

public static class DebugUtils
{
    public static T GetComponentWithErrorLogging<T>(Transform targetTransform, string componentName) where T : Component
    {
        if(targetTransform.TryGetComponent<T>(out T component))
        {
            return component;
        }
        else
        {
            Debug.LogError($"{targetTransform.gameObject.name}: Could not get {componentName}");
            return null;
        }
    }

    public static T GetComponentWithErrorLogging<T>(GameObject targetObject, string componentName) where T : Component
    {
        if(targetObject.transform.TryGetComponent<T>(out T component))
        {
            return component;
        }
        else
        {
            Debug.LogError($"{targetObject.name}: Could not get {componentName}");
            return null;
        }
    }

    /// <summary>
    /// 인자로 받은 대상이 Null인지 확인하고 Null일 경우 정해진 에러를 출력하는 함수
    /// </summary>
    /// <typeparam name="T">확인할 Type</typeparam>
    /// <param name="target">확인할 객체</param>
    /// <returns>결과값, bool</returns>
    public static bool CheckIsNullWithErrorLogging<T>(T target)
    {
        if (target == null)
        {
            Debug.LogError($"{typeof(T).Name} is null");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 인자로 받은 대상이 Null인지 확인하고 Null일 경우 인자로 받은 메시지 출력하는 함수
    /// </summary>
    /// <typeparam name="T">확인할 타입</typeparam>
    /// <param name="target">확인할 객체</param>
    /// <param name="debugMessage">출력할 메시지</param>
    /// <returns></returns>
    public static bool CheckIsNullWithErrorLogging<T>(T target, string debugMessage)
    {
        if (target == null)
        {
            Debug.LogError(debugMessage);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 인자로 받은 대상이 Null인지 확인하고 Null일 경우 정해진 객체를 가지고 있는 게임오브젝트를 포함한 에러를 출력하는 함수
    /// </summary>
    /// <typeparam name="T">확인할 Type</typeparam>
    /// <param name="target">확인할 대상</param>
    /// <param name="parent">해당 컴포넌트를 가지는 게임오브젝트</param>
    /// <returns></returns>
    public static bool CheckIsNullWithErrorLogging<T>(T target, GameObject parent)
    {
        if (target == null)
        {
            Debug.LogError($"{parent.name}'s {typeof(T).Name} is null");
            return true;
        }

        return false;
    }
}
