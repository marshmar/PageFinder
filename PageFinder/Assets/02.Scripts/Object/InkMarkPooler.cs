using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Class responsible for pooling inkmark objects.
/// </summary>
public class InkMarkPooler : Singleton<InkMarkPooler> 
{
    public GameObject inkMarkPrefab;
    public int maxPoolSize = 40;
    public int defaultPoolCapacity = 10;

    private ObjectPool<InkMark> pool;

    public ObjectPool<InkMark> Pool { get => pool; set => pool = value; }

    public override void Awake()
    {
        base.Awake();
        pool = new ObjectPool<InkMark>(
            CreatedPooledItem, 
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            defaultPoolCapacity,
            maxPoolSize);
    }

    // Initial object allocation event function when creating an object pool
    private InkMark CreatedPooledItem()
    {
        var inkMarkObject = Instantiate(inkMarkPrefab, this.transform);
        inkMarkObject.name = $"InkMark{inkMarkObject.transform.GetSiblingIndex()}";
        inkMarkObject.SetActive(false);
        return inkMarkObject.GetComponent<InkMark>();
    }

    // Return a used object to the object pool
    private void OnReturnedToPool(InkMark inkMark)
    {
        inkMark.gameObject.SetActive(false);
    }

    // Take objects from the object pool
    private void OnTakeFromPool(InkMark inkMark)
    {
        inkMark.gameObject.SetActive(true);
    }

    // Destroy an object pool object
    private void OnDestroyPoolObject(InkMark inkMark)
    {
        Destroy(inkMark.gameObject);
    }
}