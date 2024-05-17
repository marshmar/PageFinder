using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    /*
     * 그 외 필요한 변수들 설정
     */
    protected Transform tr;
    protected Animator anim;
    protected Rigidbody rigid;
    protected UtilsManager utilsManager;

    // Start is called before the first frame update
    public virtual void Start()
    {
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
