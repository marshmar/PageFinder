using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: Player
{
    #region Move

    private Vector3 moveDir;
    [SerializeField]
    private MoveJoystick moveJoystick;

    #endregion

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // 키보드 이동
        KeyboardControl();
        // 조이스틱 이동
        JoystickControl();

        anim.SetFloat("Movement", moveDir.magnitude);
    }

    private void KeyboardControl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = new Vector3(h, 0, v).normalized;
        if (h != 0 || v != 0)
        {
           Move(moveDir);
        }
    }

    private void JoystickControl()
    {
        // 이동 조이스틱의 x, y 값 읽어오기
        float x = moveJoystick.Horizontal();
        float y = moveJoystick.Vertical();

        if(x!= 0 || y != 0)
        {
            moveDir = new Vector3(x, 0, y).normalized;
            Move(moveDir);
        }
    }

    private void Move(Vector3 moveDir)
    {
        tr.Translate(modelTr.forward * moveSpeed * Time.deltaTime);
        //tr.position += moveDir * moveSpeed * Time.deltaTime;
        TurnToDirection(moveDir);
    }
}
