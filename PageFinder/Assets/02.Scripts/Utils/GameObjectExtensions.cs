using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetComponentWithErrorLogging<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.transform.TryGetComponent<T>(out T component))
        {
            return component;
        }
        else
        {
            Debug.LogError($"{gameObject.name}'s {typeof(T).Name} missing!");
            return null;
        }
    }
}
