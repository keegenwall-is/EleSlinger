using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;
    public float sprintSpeedMultiplier;
    public float rotateSpeed;
    public Rigidbody rb;
    public float dashSpeed;
    public float maxSprint;
    public Image sprintMeter;

    private Vector3 moveDir;
    private CharacterBase baseScript;
    private bool isDashing = false;
    private float baseSpeed;
    private float originalSpeed;
    private bool isSprinting;
    private float currentSprint;
    private bool speedBuffed;
    private Keyboard keyboard;
    private Gamepad controller;
    private Vector3 initialSprintScale;

    [SerializeField] private AnimationCurve dashCurve = AnimationCurve.Linear(0, 1, 1, 0);

    // Start is called before the first frame update
    void Start()
    {
        currentSprint = maxSprint;
        baseScript = GetComponent<CharacterBase>();
        rb = GetComponent<Rigidbody>();
        baseSpeed = moveSpeed;
        initialSprintScale = sprintMeter.rectTransform.localScale;

        if (baseScript.thisController is Keyboard thisKeyboard)
        {
            keyboard = thisKeyboard;
        }
        else if (baseScript.thisController is Gamepad thisController)
        {
            controller = thisController;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.canMove)
        {
            if (keyboard != null)
            {
                if (baseScript.canMove)
                {
                    if (keyboard.spaceKey.wasPressedThisFrame && !isDashing && baseScript.GetState() != CharacterBase.playerState.Idle && baseScript.GetState() != CharacterBase.playerState.UsingItem)
                    {
                        StartCoroutine(Dash());
                        return;
                    }

                    if (currentSprint >= 0)
                    {
                        if (keyboard.leftShiftKey.wasPressedThisFrame && baseScript.GetState() == CharacterBase.playerState.Running)
                        {
                            if (!speedBuffed)
                            {
                                originalSpeed = moveSpeed;
                                moveSpeed *= sprintSpeedMultiplier;
                            }
                            isSprinting = true;
                            sprintMeter.enabled = true;
                        }
                    }
                }
            }
            else if (controller != null)
            {
                if (controller.buttonSouth.wasPressedThisFrame && !isDashing && baseScript.GetState() != CharacterBase.playerState.Idle && baseScript.GetState() != CharacterBase.playerState.UsingItem)
                {
                    StartCoroutine(Dash());
                    return;
                }

                if (currentSprint >= 0)
                {
                    if (controller.buttonWest.wasPressedThisFrame && baseScript.GetState() == CharacterBase.playerState.Running)
                    {
                        if (!speedBuffed)
                        {
                            originalSpeed = moveSpeed;
                            moveSpeed *= sprintSpeedMultiplier;
                        }
                        isSprinting = true;
                        sprintMeter.enabled = true;
                    }
                }
            }

            //if (baseScript.GetState() != CharacterBase.playerState.Dashing)
            //{
            MoveDirection();
            //}
        }

        if (keyboard != null)
        {
            if (keyboard.leftShiftKey.wasReleasedThisFrame)
            {
                if (!speedBuffed)
                {
                    DecreaseSpeed();
                }
                isSprinting = false;
            }
        }
        else if (controller != null)
        {
            if (controller.buttonWest.wasReleasedThisFrame)
            {
                if (!speedBuffed)
                {
                    DecreaseSpeed();
                }
                isSprinting = false;
            }
        }

        if (isSprinting)
        {
            if (!speedBuffed)
            {
                currentSprint -= Time.deltaTime;
                if (currentSprint < 0)
                {
                    currentSprint = 0;
                    DecreaseSpeed();
                    isSprinting = false;
                }
            }
            else
            {
                currentSprint = maxSprint;
            }

            sprintMeter.transform.rotation = Camera.main.transform.rotation;
            sprintMeter.fillAmount = currentSprint / maxSprint;
            float distance = Vector3.Distance(sprintMeter.transform.position, Camera.main.transform.position);
            float clampedDistance = Mathf.Clamp(distance, 2f, 35f);

            sprintMeter.rectTransform.localScale = initialSprintScale * clampedDistance * 0.05f;
        }
        else if (currentSprint <= maxSprint)
        {
            currentSprint += Time.deltaTime;
            sprintMeter.transform.rotation = Camera.main.transform.rotation;
            float distance = Vector3.Distance(sprintMeter.transform.position, Camera.main.transform.position);
            float clampedDistance = Mathf.Clamp(distance, 2f, 35f);

            sprintMeter.rectTransform.localScale = initialSprintScale * clampedDistance * 0.05f;
            sprintMeter.fillAmount = currentSprint / maxSprint;
        }
        else if (currentSprint >= maxSprint)
        {
            currentSprint = maxSprint;
            sprintMeter.enabled = false;
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
            if (baseScript.GetState() != CharacterBase.playerState.TakingHit && baseScript.GetState() != CharacterBase.playerState.Falling && !isDashing)
            {
                rb.linearVelocity = new Vector3(0, 0, 0);
            }
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
            if (baseScript.GetState() != CharacterBase.playerState.UsingItem && !isDashing)
            {
                baseScript.SetState(CharacterBase.playerState.Running);
            }
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
        else
        {
            if (baseScript.GetState() != CharacterBase.playerState.UsingItem && !isDashing)
            {
                baseScript.SetState(CharacterBase.playerState.Idle);
            }
        }
    }

    void Move()
    {
        Vector3 newVelocity = moveDir * moveSpeed;
        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
    }

    IEnumerator Dash()
    {
        baseScript.SetState(CharacterBase.playerState.Dashing);
        isDashing = true;
        transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

        yield return new WaitForSeconds(0.1f);

        originalSpeed = moveSpeed;
        float targetSpeed = baseSpeed * dashSpeed;
        if (speedBuffed)
        {
            targetSpeed *= sprintSpeedMultiplier;
        }
        float dashDuration = baseScript.anim.GetCurrentAnimatorStateInfo(0).length - 0.2f;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashDuration;

            float curveValue = dashCurve.Evaluate(t);

            moveSpeed = Mathf.Lerp(originalSpeed, targetSpeed, curveValue);

            yield return null;
        }

        moveSpeed = originalSpeed;

        if (baseScript.GetState() == CharacterBase.playerState.Dashing)
        {
            baseScript.SetState(CharacterBase.playerState.Idle);
        }
        isDashing = false;
    }

    public void IncreaseSpeed(float speedMultiplier)
    {
        originalSpeed = baseSpeed * speedMultiplier;
        moveSpeed = baseSpeed * speedMultiplier;
        speedBuffed = true;
    }

    public void DecreaseSpeed()
    {
        originalSpeed = baseSpeed;
        moveSpeed = baseSpeed;
        speedBuffed = false;
    }
}
