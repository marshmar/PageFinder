using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum InkType
{
    RED,
    GREEN,
    BLUE,   
    FIRE,   // Shadow
    MIST,   // Transparency
    SWAMP   // Rotation
}
public class InkMark : MonoBehaviour
{
    #region Variables
    public float duration;
    public float fusionDuration;

    private bool isAbleFusion = true;
    private bool isFusioned = false;
    private bool fadeOut = false;
    private bool inTrigger = false;
    private bool decreasingTransparency;
    private float spawnTime = 0f;
    
    private InkType currType;
    private InkMarkType currInkMarkType;
    private SpriteRenderer spriteRenderer;
    private SpriteMask spriteMask;
    private PlayerState playerState;
    #endregion

    #region Properties
    public InkType CurrType { get => currType; set => currType = value; }
    /*public float SpawnTime { get => spawnTime; set
        {
            spawnTime = value;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
        }
    }*/
    public bool IsFusioned { get => isFusioned; set => isFusioned = value; }
    public bool FadeOut { get => fadeOut; set => fadeOut = value; }
    public InkMarkType CurrInkMarkType { get => currInkMarkType; set => currInkMarkType = value; }
    public bool IsAbleFusion { get => isAbleFusion; set => isAbleFusion = value; }
    #endregion


    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteMask = GetComponentInChildren<SpriteMask>();
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerState");
    }

    private void Start()
    {
        if (currType == InkType.FIRE)
        {
            StartCoroutine(PlayEffect(0));
        }
        if (currType == InkType.SWAMP)
        {
            StartCoroutine(PlayEffect(1));
        }
        if (currType == InkType.MIST)
        {
            StartCoroutine(PlayEffect(2));
        }
    }

    IEnumerator PlayEffect(int index)
    {
        yield return new WaitForSeconds(0.3f);
        InkMarkSetter.Instance.SetEffect(index, this.transform);
    }

    public void SetInkMarkData(InkMarkType inkMarkType, InkType inkType, bool addCollider = true)
    {
        currInkMarkType = inkMarkType;
        currType = inkType;
        SetInkMark(addCollider);
    }

    public void SetSynthesizedInkMarkData(InkMarkType inkMarkType, InkType inkType)
    {
        currInkMarkType = inkMarkType;
        currType = inkType;
        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(currInkMarkType, transform, ref duration);
    }

    private void OnDisable()
    {
        ResetInkMark();
    }

    private void ResetInkMark()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        spawnTime = 0f;
        duration = 0f;
        isFusioned = false;
        decreasingTransparency = false;
        isAbleFusion = true;
        fadeOut = false;
        inTrigger = false;

        Destroy(this.GetComponent<Collider>());
    }

    private void Update()
    {
        if (!this.isActiveAndEnabled) return;
        spawnTime += Time.deltaTime;
        if ((spawnTime >= duration - 1.0f && !decreasingTransparency) || (fadeOut && !decreasingTransparency))
        {
            decreasingTransparency = true;
            RemoveSynthesizeEffect();
            StartCoroutine(DecreaseTransparency());
        }
    }

    private bool CheckInkMarkFusionCondition(InkMark otherMark)
    {
        if (spawnTime > otherMark.spawnTime || !isAbleFusion || !otherMark.IsAbleFusion || isFusioned || otherMark.isFusioned || this.currType == otherMark.currType) return false;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("INKMARK") && !inTrigger)
        {
            inTrigger = true;
            if (other.TryGetComponent<InkMark>(out InkMark otherMark))
            {
                if (CheckInkMarkFusionCondition(otherMark))
                {
                    Debug.Log("Is Fusionable");
                    var myCollider = GetComponent<Collider>();
                    Debug.Log($"{currInkMarkType}, {otherMark.CurrInkMarkType} Intersection Check");
                    switch (currInkMarkType)
                    {
                        case InkMarkType.DASH:
                            if (otherMark.CurrInkMarkType == InkMarkType.DASH)
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenRectangles(myCollider, other))
                                {
                                    Debug.Log("Rectangle, Rectangle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject);
                                    SetStatusFusioned(otherMark);
                                }
                            }
                            else
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenRectangleCircle(myCollider, other))
                                {
                                    Debug.Log("Rectangle, Circle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject);
                                    SetStatusFusioned(otherMark);
                                }
                            }
                            break;
                        default:
                            if (otherMark.CurrInkMarkType == InkMarkType.DASH)
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenRectangleCircle(other, myCollider))
                                {
                                    Debug.Log("Skill, Dash Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject);
                                    SetStatusFusioned(otherMark);
                                }
                            }
                            else
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenCircles(myCollider, other))
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

        if (other.CompareTag("ENEMY") && !decreasingTransparency)
        {
            EnemyBuff enemyBuff = other.GetComponent<EnemyBuff>();
            if (enemyBuff == null) return;

            switch (currType)
            {
                // Add InkMarkFire effect, 100 is InkMarkFireBuff's ID
                case InkType.FIRE:
                    enemyBuff.AddBuff(new BuffData(BuffType.BuffType_Tickable, 100, 0f, targets: new List<Component>() { playerState, other.GetComponent<Enemy>() }));
                    break;
                // Add InkMarkMist effect, 102 is InkMarkMistBuff's ID
                case InkType.MIST:
                    enemyBuff.AddBuff(new BuffData(BuffType.BuffType_Permanent, 102, 0f, targets: new List<Component>() { other.GetComponent<Enemy>() }));
                    break;
            }
        }

        if (other.CompareTag("PLAYER") && !decreasingTransparency)
        {
            PlayerBuff playerBuff = other.GetComponent<PlayerBuff>();
            if(playerBuff == null)
            {
                Debug.LogError("Failed To GetComponent PlayerBuff");
                return;
            }

            switch (currType)
            {
                case InkType.SWAMP:
                    // Add InkMarkSwamp effect, 101 is InkMarkSwampBuff's ID
                    playerBuff.AddBuff(new BuffData(BuffType.BuffType_Tickable, 101, 0f, targets: new List<Component>() { playerState }));
                    break;
                case InkType.MIST:
                    // Add InkMarkMist effect, 102 is InkMarkSwampBuff's ID
                    playerBuff.AddBuff(new BuffData(BuffType.BuffType_Tickable, 102, 0f, targets: new List<Component>() { playerState }));
                    Debug.Log("Send event");
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

            switch (currType)
            {
                // Remove InkMarkFire effect, 100 is InkMarkFireBuff's ID
                case InkType.FIRE:
                    enemyBuff.RemoveBuff(100);
                    break;
                // Remove InkMarkMist effect, 102 is InkMarkMistBuff's ID
                case InkType.MIST:
                    enemyBuff.RemoveBuff(102);
                    break;
            }
        }

        if (other.CompareTag("PLAYER"))
        {
            PlayerBuff playerBuff = other.GetComponent<PlayerBuff>();
            if (playerBuff == null)
            {
                Debug.LogError("Failed To GetComponent PlayerBuff");
                return;
            }

            switch (currType)
            {
                case InkType.SWAMP:
                    // Remove InkMarkMist effect, 101 is InkMarkSwampBuff's ID
                    playerBuff.RemoveBuff(101);
                    break;
            }
        }
    }


    public void SetInkMark(bool addCollider = true)
    {
        if(spriteRenderer == null) Debug.Log(gameObject.name + "'s spriteRenderer is null");

        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(currInkMarkType, transform, ref duration);
        if(addCollider) AddCollider();

        if(!InkMarkSetter.Instance.SetInkMarkSprite(currInkMarkType, currType, spriteRenderer, spriteMask))
        {
            Debug.LogError("Failed to assign ink mark sprite");
        }
    }

    public void AddCollider()
    {
        InkMarkSetter.Instance.AddCollider(currInkMarkType, transform);
    }

    public IEnumerator DecreaseTransparency()
    {
        float time = 0.0f;

        while (time <= 1.0f)
        {
            time += Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1 - time);
            if (1 - time <= 0.3f) IsAbleFusion = false;

            yield return null;
        }

        if(IsAbleFusion) IsAbleFusion = false;
        InkMarkPooler.Instance.Pool.Release(this);
        yield break;
    }

    private void SetStatusFusioned(InkMark otherMark)
    {
        isAbleFusion = false;
        otherMark.IsAbleFusion = false;
        IsFusioned = true;
        otherMark.isFusioned = true;
    }

    private void RemoveSynthesizeEffect()
    {
        // 2.5 is Synthesized InkMark's SphereCollider's radius
        Collider[] colls = Physics.OverlapSphere(transform.position, 2.5f);
        foreach(Collider coll in colls)
        {
            switch (currType)
            {
                case InkType.FIRE:
                    if(coll.TryGetComponent<EnemyBuff>(out EnemyBuff enemyBuff))
                        enemyBuff.RemoveBuff(100);
                    break;
                case InkType.SWAMP:
                    if(coll.TryGetComponent<PlayerBuff>(out PlayerBuff playerBuff ))
                        playerBuff.RemoveBuff(101);
                    break;
                case InkType.MIST:
                    if (coll.TryGetComponent<EnemyBuff>(out EnemyBuff enemyBuff2))
                        enemyBuff2.RemoveBuff(102);
                    break;
            }
        }
    }
}