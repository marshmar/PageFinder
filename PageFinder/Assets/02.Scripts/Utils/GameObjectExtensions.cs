using UnityEngine;

public static class GameObjectExtensions
{
/*    public static T GetComponentSafe<T>(this Component component) where T : Component
    {
        if (component.TryGetComponent<T>(out T comp))
        {
            return comp;
        }
        else
        {
            Debug.LogError($"{component.gameObject.name}'s {typeof(T).Name} missing!");
            if (component is Behaviour behaviour)
                behaviour.enabled = false;
            return null;
        }
    }

    public static T GetComponentInChildrenSafe<T>(this Component component) where T : Component
    {
        T result = component.GetComponentInChildren<T>();

        if (result == null)
        {
            Debug.LogError($"{component.gameObject.name} has no children with component of type {typeof(T).Name}.");
            if (component is Behaviour behaviour)
                behaviour.enabled = false;
        }

        return result;
    }*/
}
