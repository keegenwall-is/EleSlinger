using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : BaseCharacter
{
    public float moveSpeed;
    public float rotateSpeed;

    private Rigidbody rb;
    private Vector3 moveDir;
    private InputDevice thisController;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thisController = Keyboard.current;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveDirection();
    }

    void FixedUpdate()
    {
        Move();
    }

    void MoveDirection()
    {
        float moveX = 0;
        float moveZ = 0;

        //gets vertical and horizontal input from the input device
        if (thisController is Keyboard keyboard)
        {
            moveZ = keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0;
            moveX = keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0;
        }
        else if (thisController is Gamepad controller)
        {
            moveZ = controller.leftStick.up.isPressed ? 1 : controller.leftStick.down.isPressed ? -1 : 0;
            moveX = controller.leftStick.right.isPressed ? 1 : controller.leftStick.left.isPressed ? -1 : 0;
        }

        moveDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveZ == 0f && moveX == 0f)
        {
            anim.Play("CatIdle");
        }
        else
        {
            anim.Play("CatRun");
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        Vector3 newVelocity = moveDir * moveSpeed;
        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
    }

}
