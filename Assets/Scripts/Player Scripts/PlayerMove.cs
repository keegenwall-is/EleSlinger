using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public Rigidbody rb;

    private Vector3 moveDir;
    private CharacterBase baseScript;
    private bool isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.canMove)
        {
            if (baseScript.thisController is Keyboard keyboard)
            {
                if (keyboard.spaceKey.wasPressedThisFrame && !isDashing && baseScript.GetState() != CharacterBase.playerState.Idle)
                {
                    StartCoroutine(Dash());
                    return;
                }
            }
            else if (baseScript.thisController is Gamepad controller)
            {
                if (controller.buttonSouth.wasPressedThisFrame && !isDashing && baseScript.GetState() != CharacterBase.playerState.Idle)
                {
                    StartCoroutine(Dash());
                    return;
                }
            }

            if (baseScript.GetState() != CharacterBase.playerState.Dashing)
            {
                MoveDirection();
            }
        }
    }

    void FixedUpdate()
    {
        if (baseScript.canMove)
        {
            Move();
        }
        else
        {
            if (baseScript.GetState() != CharacterBase.playerState.TakingHit)
            rb.velocity = new Vector3(0, 0, 0);
        }
    }

    void MoveDirection()
    {

        float moveX = 0;
        float moveZ = 0;

        //gets vertical and horizontal input from the input device
        if (baseScript.thisController is Keyboard keyboard)
        {
            moveZ = keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0;
            moveX = keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0;
        }
        else if (baseScript.thisController is Gamepad controller)
        {
            Vector2 stickInput = controller.leftStick.ReadValue();
            moveX = stickInput.x;
            moveZ = stickInput.y;
        }

        moveDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveZ != 0f || moveX != 0f)
        {
            baseScript.SetState(CharacterBase.playerState.Running);
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
        else
        {
            baseScript.SetState(CharacterBase.playerState.Idle);
        }
    }

    void Move()
    {
        Vector3 newVelocity = moveDir * moveSpeed;
        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
    }

    IEnumerator Dash()
    {
        baseScript.SetState(CharacterBase.playerState.Dashing);
        isDashing = true;
        moveSpeed = moveSpeed * 2;
        transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(baseScript.anim.GetCurrentAnimatorStateInfo(0).length - 0.1f);

        baseScript.SetState(CharacterBase.playerState.Idle);
        isDashing = false;
        moveSpeed = moveSpeed / 2;
    }

}
