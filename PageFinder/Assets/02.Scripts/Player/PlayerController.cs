using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: Player
{
    #region Move
    [SerializeField]
    private Vector3 moveDir;
    [SerializeField]
    private VirtualJoystick virtualJoystick;

    #endregion

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // 이동 조이스틱의 x, y 값 읽어오기
        float x = virtualJoystick.Horizontal();
        float y = virtualJoystick.Vertical();


        moveDir = new Vector3(x, 0, y).normalized;
        if(x != 0 || y != 0)
        {   
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            TurnToDirection(moveDir);
        }
        anim.SetFloat("Movement", moveDir.magnitude);
    }

    /*// 플레이어 이동
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);
        anim.SetFloat("Movement", dir.magnitude);
    }*/

}
