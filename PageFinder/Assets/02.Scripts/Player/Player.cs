using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IPlayer
{
    /*
     * 그 외 필요한 변수들 설정
     */
    protected float moveSpeed;
    protected float maxHP;
    protected float currHP;
    protected float atk;
    protected float img;
    protected float maxMana;
    protected float currMana;
    protected float manaGain;
    protected float def;
    protected float attackSpeed;


    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    public float HP
    {
        get { return currHP; }
        set
        {
            currHP = value;
            if(currHP <= 0)
            {
                Debug.Log("Player Die");
                SceneManager.LoadScene("Title");
            }
        }
    }
    public float MoveSpeed {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
        }
    }

    
    protected Transform tr;
    protected Animator anim;
    protected Rigidbody rigid;
    protected UtilsManager utilsManager;
    protected Palette palette;

    public virtual void Awake()
    {
        palette = GameObject.FindWithTag("PLAYER").GetComponent<Palette>();
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        if(SceneManager.GetActiveScene().name == "Title")
        {
            Destroy(this.gameObject);
        }
        //playerInput = GetComponent<PlayerInput>();
        //playerInputActions = GetComponent<PlayerInputActions>();
        SetBasicStatus();
        DontDestroyOnLoad(this);
        Hasing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 CaculateDirection(Collider goalObj)
    {
        Vector3 dir = goalObj.gameObject.transform.position - tr.position;
        return dir;
    }
    public void TurnToDirection(Vector3 dir)
    {
        tr.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
    }

    public virtual void Hasing()
    {
        // 컴포넌트 세팅
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();

        utilsManager = UtilsManager.Instance;
    }

    // 플레이어 기본 능력치 설정
    public void SetBasicStatus()
    {
        maxHP = 100.0f;
        atk = 20.0f;
        currHP = maxHP;
        moveSpeed = 10.0f;
    }
}
