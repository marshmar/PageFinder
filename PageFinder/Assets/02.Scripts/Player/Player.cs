using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    /*
     * 그 외 필요한 변수들 설정
     */
    protected float moveSpeed = 10.0f;
    protected float maxHP;
    protected float currHP;
    protected float atk;
    protected float img;
    protected float maxMana;
    protected float currMana;
    protected float manaGain;
    protected float def;
    protected float attackSpeed;


    public float MoveSpeed { get; set; }

    
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
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();

        utilsManager = UtilsManager.Instance;
    }
}
