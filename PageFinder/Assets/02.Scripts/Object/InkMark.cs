using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum InkType
{
    Red = 0,
    Green,
    Blue,   
    Fire,   // Shadow
    Mist,   // Transparency
    Swamp,  // Rotation
    None
}
public class InkMark : MonoBehaviour
{
    #region Variables
    public float Duration;
    public float FusionDuration;

    private bool _isAbleFusion              = true;
    private bool _isFusioned                = false;
    private bool _isFadingOut               = false;
    private bool _isInTrigger               = false;
    private bool _isDecreasingTransparency  = false;
    private float _spawnTime                = 0f;
    private float _playerEnteredTime        = 0f;
    private const float FadeOutThreshold = 1.0f;
    private int _currSwampLevel;
    private InkType _inkType;
    private InkMarkType _inkMarkType;

    // Hashing
    private SpriteRenderer _spriteRenderer;
    private SpriteMask _spriteMask;
    private PlayerState _playerState;
    #endregion

    #region Properties
    public InkType CurrType { get => _inkType; set => _inkType = value; }
    /*public float SpawnTime { get => spawnTime; set
        {
            spawnTime = value;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
        }
    }*/
    public bool IsFusioned { get => _isFusioned; set => _isFusioned = value; }
    public bool IsFadingOut { get => _isFadingOut; set => _isFadingOut = value; }
    public InkMarkType InkMarkType { get => _inkMarkType; set => _inkMarkType = value; }
    public bool IsAbleFusion { get => _isAbleFusion; set => _isAbleFusion = value; }
    public bool IsDecreasingTransparency { get => _isDecreasingTransparency; }
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Initialization
    private void Awake()
    {
        _spriteRenderer = this.GetComponentInChildrenSafe<SpriteRenderer>();
        _spriteMask = this.GetComponentInChildrenSafe<SpriteMask>();
        _playerState = GameObject.FindGameObjectWithTag("PLAYER").GetComponentSafe<PlayerState>();
    }

    private void Start()
    {
        if (_inkType == InkType.Fire)
        {
            StartCoroutine(PlayEffect(0));
        }
        if (_inkType == InkType.Swamp)
        {
            StartCoroutine(PlayEffect(1));
        }
        if (_inkType == InkType.Mist)
        {
            StartCoroutine(PlayEffect(2));
        }
    }

    private void Update()
    {
        if (!this.isActiveAndEnabled) return;
        _spawnTime += Time.deltaTime;

        if ((_spawnTime >= Duration - FadeOutThreshold && !_isDecreasingTransparency) || (_isFadingOut && !_isDecreasingTransparency))
        {
            _isDecreasingTransparency = true;
            RemoveSynthesizeEffect();
            StartCoroutine(DecreaseTransparency());
        }
    }

    private void OnDisable()
    {
        ResetInkMark();
    }
    #endregion

    #region Actions
    IEnumerator PlayEffect(int index)
    {
        yield return new WaitForSeconds(0.3f);
        InkMarkSetter.Instance.SetEffect(index, this.transform);
    }

    public void SetInkMarkData(InkMarkType inkMarkType, InkType inkType, bool addCollider = true)
    {
        _inkMarkType = inkMarkType;
        _inkType = inkType;

        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(_inkMarkType, transform, ref Duration);
        if (addCollider) AddCollider();

        if (!InkMarkSetter.Instance.SetInkMarkSprite(_inkMarkType, _inkType, _spriteRenderer, _spriteMask))
        {
            Debug.LogError("Failed to assign ink mark sprite");
        }
    }

    public void SetSynthesizedInkMarkData(InkMarkType inkMarkType, InkType inkType)
    {
        _inkMarkType = inkMarkType;
        _inkType = inkType;

        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(_inkMarkType, transform, ref Duration);
    }

    private void ResetInkMark()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        _spawnTime = 0f;

        _isFusioned = false;
        _isDecreasingTransparency = false;
        _isAbleFusion = true;
        _isFadingOut = false;
        _isInTrigger = false;

        Duration = 0f;
        Destroy(this.GetComponent<Collider>());
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








    private bool CheckInkMarkFusionCondition(InkMark otherMark)
    {
        if (_spawnTime > otherMark._spawnTime || !_isAbleFusion || !otherMark.IsAbleFusion || _isFusioned || otherMark._isFusioned || this._inkType == otherMark._inkType) return false;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("INKMARK") && !_isInTrigger)
        {
            _isInTrigger = true;
            if (other.TryGetComponent<InkMark>(out InkMark otherMark))
            {
                if (CheckInkMarkFusionCondition(otherMark))
                {
                    Debug.Log("Is Fusionable");
                    var myCollider = GetComponent<Collider>();
                    const float intersectAreaThreshold = 0.25f;
                    switch (_inkMarkType)
                    {
                        
                        case InkMarkType.DASH:
                            if (otherMark.InkMarkType == InkMarkType.DASH)
                            {
                                if (GeometryUtils.CheckIntersectionBetweenRectangles(myCollider, other, intersectAreaThreshold))
                                {
                                    Debug.Log("Rectangle, Rectangle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject);
                                    SetStatusFusioned(otherMark);
                                }
                            }
                            else
                            {
                                if (GeometryUtils.CheckIntersectionBetweenRectangleCircle(myCollider, other, intersectAreaThreshold))
                                {
                                    Debug.Log("Rectangle, Circle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject);
                                    SetStatusFusioned(otherMark);
                                }
                            }
                            break;
                        default:
                            if (otherMark.InkMarkType == InkMarkType.DASH)
                            {
                                if (GeometryUtils.CheckIntersectionBetweenRectangleCircle(other, myCollider, intersectAreaThreshold))
                                {
                                    Debug.Log("Skill, Dash Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject);
                                    SetStatusFusioned(otherMark);
                                }
                            }
                            else
                            {
                                if (GeometryUtils.CheckIntersectionBetweenCircles(myCollider, other, intersectAreaThreshold))
                                {
                                    Debug.Log("Circle, Circle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject);
                                    SetStatusFusioned(otherMark);
                                }
                            }

                            break;
                    }
                }
            }
        }

        if (other.CompareTag("ENEMY") && !_isDecreasingTransparency)
        {
            EnemyBuff enemyBuff = other.GetComponent<EnemyBuff>();
            if (enemyBuff == null) return;

            switch (_inkType)
            {

                // Add InkMarkMist effect, 102 is InkMarkMistBuff's ID
                case InkType.Mist:
                    BuffData buffData = new BuffData(BuffType.BuffType_Permanent, 102, 0f, targets: new List<Component>() { other.GetComponent<Enemy>() });
                    enemyBuff.AddBuff(in buffData);
                    break;
            }
        }

        if (other.CompareTag("PLAYER") && !_isDecreasingTransparency)
        {
            PlayerBuff playerBuff = other.GetComponent<PlayerBuff>();
            if(playerBuff == null)
            {
                Debug.LogError("Failed To GetComponent PlayerBuff");
                return;
            }

            switch (_inkType)
            {
                case InkType.Swamp:
                    // Add InkMarkSwamp effect, 101 is InkMarkSwampBuff's ID
                    BuffData buffData = new BuffData(BuffType.BuffType_Tickable, 101, 0f, targets: new List<Component>() { _playerState });
                    playerBuff.AddBuff(in buffData);
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PLAYER") &&!_isDecreasingTransparency)
        {
            switch (_inkType)
            {
                case InkType.Swamp:
                    _playerEnteredTime += Time.deltaTime;
                    int newLevel = Mathf.FloorToInt((_playerEnteredTime - 1f) / 2f);
                    if (newLevel > _currSwampLevel)
                    {
                        PlayerBuff playerBuff = other.GetComponent<PlayerBuff>();
                        if (playerBuff == null)
                        {
                            Debug.LogError("Failed To GetComponent PlayerBuff");
                            return;
                        }

                        playerBuff.ChangeBuffLevel(101, newLevel);
                        _currSwampLevel = newLevel;
                    }
                    break;
                case InkType.Mist:
                    EventManager.Instance.PostNotification(EVENT_TYPE.InkMarkMist_Entered, this);
                    break;

            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            EnemyBuff enemyBuff = other.GetComponent<EnemyBuff>();
            if (enemyBuff == null) return;

            switch (_inkType)
            {
                // Remove InkMarkFire effect, 100 is InkMarkFireBuff's ID
                case InkType.Fire:
                    enemyBuff.RemoveBuff(100);
                    break;
                // Remove InkMarkMist effect, 102 is InkMarkMistBuff's ID
/*                case InkType.MIST:
                    enemyBuff.RemoveBuff(102);*/
                   // break;
            }
        }

        if (other.CompareTag("PLAYER") && _inkType == InkType.Swamp)
        {
            PlayerBuff playerBuff = other.GetComponent<PlayerBuff>();
            if (playerBuff.IsNull()) return;

            // Remove InkMarkMist effect, 101 is InkMarkSwampBuff's ID
            playerBuff.RemoveBuff(101);
        }
    }




    public void AddCollider()
    {
        InkMarkSetter.Instance.AddCollider(_inkMarkType, transform);
    }

    public IEnumerator DecreaseTransparency()
    {  
        float elapsedTimeSec = 0.0f;
        const float fadeOutTime = 1.0f;
        const float fusionDisableThreshold = 0.3f;
        float alpha;

        while (elapsedTimeSec <= fadeOutTime)
        {
            elapsedTimeSec += Time.deltaTime;
            alpha = Mathf.Clamp01(fadeOutTime - elapsedTimeSec);
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, alpha);

            if (fadeOutTime - elapsedTimeSec <= fusionDisableThreshold) IsAbleFusion = false;

            yield return null;
        }

        switch (_inkType)
        {
            case InkType.Swamp:
                AudioManager.Instance.Play(Sound.swampDeleted, AudioClipType.InkMarkSfx);
                break;
            case InkType.Mist:
                AudioManager.Instance.Play(Sound.mistDeleted, AudioClipType.InkMarkSfx);
                break;
            case InkType.Fire:
                AudioManager.Instance.Play(Sound.fireDeleted, AudioClipType.InkMarkSfx);
                break;
        }
        
        IsAbleFusion = false;
        InkMarkPooler.Instance.Pool.Release(this);
        yield break;
    }

    private void SetStatusFusioned(InkMark otherMark)
    {
        _isAbleFusion = false;
        otherMark.IsAbleFusion = false;
        IsFusioned = true;
        otherMark._isFusioned = true;
    }

    private void RemoveSynthesizeEffect()
    {
        // 2.5 is Synthesized InkMark's SphereCollider's radius
        const float sphereCollRadius = 2.5f;
        Collider[] colls = Physics.OverlapSphere(transform.position, sphereCollRadius);

        foreach(Collider coll in colls)
        {
            switch (_inkType)
            {
                case InkType.Fire:
                    if(coll.TryGetComponent<EnemyBuff>(out EnemyBuff enemyBuff))
                        enemyBuff.RemoveBuff(100);
                    break;
                case InkType.Swamp:
                    if(coll.TryGetComponent<PlayerBuff>(out PlayerBuff playerBuff ))
                        playerBuff.RemoveBuff(101);
                    break;
/*                case InkType.MIST:
                    if (coll.TryGetComponent<EnemyBuff>(out EnemyBuff enemyBuff2))
                        enemyBuff2.RemoveBuff(102);
                    break;*/
            }
        }
    }
}