using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Class responsible for pooling inkmark objects.
/// </summary>
public class InkMarkPooler : Singleton<InkMarkPooler>
{
    #region Variables
    [SerializeField] public GameObject InkMarkPrefab;
    public const int MaxPoolSize = 40;
    public const int DefaultPoolCapacity = 10;

    private ObjectPool<InkMark> _pool;
    #endregion

    #region Properties
    public ObjectPool<InkMark> Pool { get => _pool; set => _pool = value; }
    #endregion

    #region Unity Lifecycle
    public override void Awake()
    {
        base.Awake();
        _pool = new ObjectPool<InkMark>(
            CreatedPooledItem,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            DefaultPoolCapacity,
            MaxPoolSize);
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    // Initial object allocation event function when creating an object pool
    private InkMark CreatedPooledItem()
    {
        var inkMarkObject = Instantiate(InkMarkPrefab, this.transform);
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
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion






}