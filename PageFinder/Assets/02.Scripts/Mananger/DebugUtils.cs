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
            Debug.LogError($"{targetObject.name}'s {componentName} missing!");
            //Debug.LogError($"{targetObject.name}: Could not get {componentName}");
            return null;
        }
    }

    public static T GetComponent<T>(GameObject targetObject) where T : Component
    {
        if (targetObject.transform.TryGetComponent<T>(out T component))
        {
            return component;
        }
        else
        {
            Debug.LogError($"{targetObject.name}'s {component.name} missing!");
            return null;
        }
    }

    public static T[] GetComponentsInChildren<T>(GameObject targetObject) where T : Component
    {
        T[] values = null;

        values = targetObject.GetComponentsInChildren<T>();

        if(values == null || values.Length == 0)
        {
            Debug.LogError($"{targetObject.name} has no children with component of type {typeof(T).Name}.");
        }

        return values;
    }

    /// <summary>
    /// ���ڷ� ���� ����� Null���� Ȯ���ϰ� Null�� ��� ������ ������ ����ϴ� �Լ�
    /// </summary>
    /// <typeparam name="T">Ȯ���� Type</typeparam>
    /// <param name="target">Ȯ���� ��ü</param>
    /// <returns>�����, bool</returns>
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
    /// ���ڷ� ���� ����� Null���� Ȯ���ϰ� Null�� ��� ���ڷ� ���� �޽��� ����ϴ� �Լ�
    /// </summary>
    /// <typeparam name="T">Ȯ���� Ÿ��</typeparam>
    /// <param name="target">Ȯ���� ��ü</param>
    /// <param name="debugMessage">����� �޽���</param>
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
    /// ���ڷ� ���� ����� Null���� Ȯ���ϰ� Null�� ��� ������ ��ü�� ������ �ִ� ���ӿ�����Ʈ�� ������ ������ ����ϴ� �Լ�
    /// </summary>
    /// <typeparam name="T">Ȯ���� Type</typeparam>
    /// <param name="target">Ȯ���� ���</param>
    /// <param name="parent">�ش� ������Ʈ�� ������ ���ӿ�����Ʈ</param>
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
