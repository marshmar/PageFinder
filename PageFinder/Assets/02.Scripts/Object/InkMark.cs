using System.Collections;
using UnityEngine;

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
    private float spawnTime;
    private bool isFusioned;
    private bool isPlayerInTrigger;

    private bool isAbleFusion;
    private bool isOtherMarkInTrigger;
    private InkType currType;
    private InkMarkType currInkMarkType;

    private SpriteRenderer spriterenderer;
    private SpriteMask spriteMask;
    private PlayerState playerState;
    private bool decreasingTransparency;
    private Coroutine transparencyCoroutine;

    private float inkFusionAreaThreshold = 0.25f;
    #endregion

    #region Properties
    public InkType CurrType { get => currType; set => currType = value; }
    public float SpawnTime { get => spawnTime; set
        {
            spawnTime = value;
            spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1.0f);
        }
    }
    public bool IsFusioned { get => isFusioned; set => isFusioned = value; }
    public bool IsPlayerInTrigger { get => isPlayerInTrigger; set => isPlayerInTrigger = value; }
    public InkMarkType CurrInkMarkType { get => currInkMarkType; set => currInkMarkType = value; }
    public bool IsAbleFusion { get => isAbleFusion; set => isAbleFusion = value; }
    #endregion


    private void Awake()
    {
        isAbleFusion = true;
        isFusioned = false;
        IsPlayerInTrigger = false;
        spawnTime = 0.0f;

        spriterenderer = GetComponentInChildren<SpriteRenderer>();
        spriteMask = GetComponentInChildren<SpriteMask>();

        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerState");
    }

    public void SetInkMarkData(InkMarkType inkMarkType, InkType inkType)
    {
        currInkMarkType = inkMarkType;
        currType = inkType;
        SetInkMark();
    }

    private void OnDestroy()
    {
        //playerInkMagicControllerScr.InkMarks.Remove(this);
    }

    private void OnDisable()
    {
        ResetInkMark();
    }

    private void ResetInkMark()
    {
        spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1f);
        spawnTime = 0f;
        duration = 0f;
        isFusioned = false;
        isPlayerInTrigger = false;
        decreasingTransparency = false;
        isAbleFusion = true;

        Destroy(this.GetComponent<Collider>());
    }

    private void Update()
    {
        if (!this.isActiveAndEnabled) return;
        spawnTime += Time.deltaTime;
        if (spawnTime >= duration - 1.0f && !decreasingTransparency)
        {
            isAbleFusion = false;
            decreasingTransparency = true;
            StartCoroutine(DecreaseTransparency());
        }
    }

    private bool CheckInkMarkFusionCondition(InkMark otherMark)
    {
        Debug.Log($"{spawnTime},{otherMark.spawnTime},{isAbleFusion},{otherMark.isAbleFusion},{IsFusioned},{otherMark.IsFusioned},{this.currType},{otherMark.currType}");
        //if (spawnTime > otherMark.spawnTime || !isAbleFusion || !otherMark.IsAbleFusion || isFusioned || otherMark.isFusioned || this.currType == otherMark.currType) return false;
        if (spawnTime > otherMark.spawnTime || !isAbleFusion || !otherMark.IsAbleFusion || isFusioned || otherMark.isFusioned) return false;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("INKMARK"))
        {
            if (other.TryGetComponent<InkMark>(out InkMark otherMark))
            {
                Debug.Log("GetComponent InkMark Success");
                if (CheckInkMarkFusionCondition(otherMark))
                {
                    Debug.Log("Is Fusionable");
                    Collider myCollider = GetComponent<Collider>();
                    switch (currInkMarkType)
                    {
                        case InkMarkType.DASH:
                            if (otherMark.CurrInkMarkType == InkMarkType.DASH)
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenRectangles(myCollider, other))
                                {
                                    Debug.Log("Rectangle, Rectangle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject, 600);
                                }
                            }
                            else
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenRectangleCircle(myCollider, other))
                                {
                                    Debug.Log("Rectangle, Circle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject, 600);
                                }
                            }
                            break;
                        default:
                            if (otherMark.CurrInkMarkType == InkMarkType.DASH)
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenRectangleCircle(other, myCollider))
                                {
                                    Debug.Log("Rectangle, Rectangle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject, 600);
                                }
                            }
                            else
                            {
                                if (InkMarkSetter.Instance.CheckIntersectionBetweenCircles(myCollider, other))
                                {
                                    Debug.Log("Circle, Circle Collision");
                                    InkMarkSynthesis.Instance.Synthesize(myCollider.gameObject, other.gameObject, 600);
                                }
                            }

                            break;
                    }
                }
            }
        }
    }

    public void SetInkMark()
    {
        if(spriterenderer == null) Debug.Log(gameObject.name + "'s spriterenderer is null");

        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(currInkMarkType, transform, ref duration);
        InkMarkSetter.Instance.AddCollider(currInkMarkType, transform);

        if(!InkMarkSetter.Instance.SetInkMarkSprite(currInkMarkType, currType, spriterenderer, spriteMask))
        {
            Debug.LogError("Failed to assign ink mark sprite");
        }
    }

    public IEnumerator DecreaseTransparency()
    {
        float time = 0.0f;

        while (time <= 1.0f)
        {
            time += Time.deltaTime;
            spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1 - time);

            yield return null;
        }

        if (spawnTime >= duration) InkMarkPooler.Instance.Pool.Release(this);
        yield break;
    }
}