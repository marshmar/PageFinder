using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler<T, K> : Singleton<K> where T : Component where K : Component
{
    #region Variables
    public int maxPoolSize;
    public int defaultPoolCapacity;
    private ObjectPool<T> pool;

    [SerializeField] private GameObject poolPrefab;
    #endregion


    public ObjectPool<T> Pool { get => pool; set => pool = value; }

    public override void Awake()
    {
        base.Awake();
        CreatePool();
    }
    
    protected void CreatePool()
    {
        pool = new ObjectPool<T>(CreatedPooledItem, OnTakeFromPool,
        OnReturnedToPool,OnDestroyPoolObject, true, defaultPoolCapacity, maxPoolSize);
    }

    // Initial object allocation event function when creating an object pool
    protected virtual T CreatedPooledItem()
    {
        GameObject pooledObject = Instantiate(poolPrefab, this.transform);
        pooledObject.name = $"{pooledObject.name}{pooledObject.transform.GetSiblingIndex()}";
        pooledObject.SetActive(false);
        return pooledObject.GetComponent<T>();
    }

    // Return a used object to the object pool
    private void OnReturnedToPool(T component)
    { 
        component.gameObject.SetActive(false);
    }

    // Take objects from the object pool
    private void OnTakeFromPool(T component)
    {
        component.gameObject.SetActive(true);
    }

    // Destroy an object pool object
    private void OnDestroyPoolObject(T component)
    {
        Destroy(component.gameObject);
    }
}
