using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetComponentSafe<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.TryGetComponent<T>(out T comp))
        {
            return comp;
        }
        else
        {
            Debug.LogError($"{gameObject.name}'s {typeof(T).Name} missing!");
            gameObject.SetActive(false);
            return null;
        }
    }

    public static T GetComponentInChildrenSafe<T>(this GameObject gameObject) where T : Component
    {
        T result = gameObject.GetComponentInChildren<T>();

        if (result == null)
        {
            Debug.LogError($"{gameObject.name} has no children with component of type {typeof(T).Name}.");
            gameObject.SetActive(false);
        }

        return result;
    }

    public static bool IsNull(this GameObject gameObject)
    {
        if(gameObject == null)
        {
            Debug.LogError($"{gameObject.name} is missing.");
            return true;
        }

        return false;
    }
}
