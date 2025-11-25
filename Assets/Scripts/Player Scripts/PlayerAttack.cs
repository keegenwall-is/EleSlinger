using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private CharacterBase baseScript;
    private PlayerMove moveScript;
    private Animator anim;
    private bool isAiming;
    private GameObject thisCharge;
    private GameObject thisProjectile;
    private float projSize;
    private Vector3 indicatorStartSize;
    private bool hasSpecialAttack;
    private Vector3 projSpawnPoint;

    public float animCut;
    public GameObject projectile;
    public GameObject charge;
    public GameObject magicSpawner;
    public GameObject indicator;
    public float aimSpeed;
    public float chargeSpeed;
    public float maxSize;
    public float projSpawnHeight;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
        moveScript = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks the kind of controller the player is using
        if (baseScript.thisController is Keyboard keyboard)
        {
            //Starts the attack if the player is idle or running and presses the attack button
            if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
            {
                if (keyboard.pKey.wasPressedThisFrame)
                {
                    baseScript.SetState(CharacterBase.playerState.Attacking);
                }
            }

            //Resume the Animation if the player releases the attack button
            if (baseScript.GetState() == CharacterBase.playerState.Attacking)
            {
                if (keyboard.pKey.wasReleasedThisFrame)
                {
                    ResumeAnim();
                }
            }

            //The player can aim if they are charging
            if (isAiming)
            {
                float moveX = 0;
                float moveZ = 0;
                moveZ = keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0;
                moveX = keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0;
                AimMovement(moveX, moveZ);
            }
        }
        else if (baseScript.thisController is Gamepad controller)
        {
            if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
            {
                if (controller.buttonEast.wasPressedThisFrame)
                {
                    baseScript.SetState(CharacterBase.playerState.Attacking);
                }
            }

            //Resume the Animation if the player releases the attack button
            if (baseScript.GetState() == CharacterBase.playerState.Attacking)
            {
                if (controller.buttonEast.wasReleasedThisFrame)
                {
                    if (isAiming)
                    {
                        ResumeAnim();
                    }
                }
            }

            if (isAiming)
            {
                float moveX = 0;
                float moveZ = 0;
                Vector2 stickInput = controller.leftStick.ReadValue();
                moveX = stickInput.x * 0.5f;
                moveZ = stickInput.y * 0.5f;
                AimMovement(moveX, moveZ);
            }
        }
    }

    public void Aim()
    {
        if (baseScript.thisController is Keyboard keyboard)
        {
            if (keyboard.pKey.isPressed)
            {
                anim.speed = 0f;
                isAiming = true;
                thisCharge = Instantiate(charge, magicSpawner.transform.position, transform.rotation, magicSpawner.transform);
                indicator.SetActive(true);
            }
        }
        else if (baseScript.thisController is Gamepad controller)
        {
            if (controller.buttonEast.isPressed)
            {
                anim.speed = 0f;
                isAiming = true;
                thisCharge = Instantiate(charge, magicSpawner.transform.position, transform.rotation, magicSpawner.transform);
                indicator.SetActive(true);
            }
        }
    }

    private void AimMovement(float moveX, float moveZ)
    {
        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        if (moveZ != 0f || moveX != 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, aimSpeed * Time.deltaTime);
        }
        projSize += chargeSpeed * Time.deltaTime;
        if (projSize >= maxSize)
        {
            projSize = maxSize;
        }
        thisCharge.transform.localScale = new Vector3(projSize / 150, projSize / 150, projSize / 150);
        indicator.transform.localScale = new Vector3(projSize, projSize, projSize);
    }

    public void ResumeAnim()
    {
        anim.speed = 1f;
        isAiming = false;
        indicator.SetActive(false);
        indicator.transform.localScale = indicatorStartSize;
    }

    public void SpawnProjectile()
    {
        projSpawnPoint = magicSpawner.transform.position;
        projSpawnPoint.y = projSpawnHeight;
        thisProjectile = Instantiate(projectile, projSpawnPoint, transform.rotation);
        ProjectileBehaviour projScript = thisProjectile.GetComponent<ProjectileBehaviour>();
        projScript.SetThrower(gameObject);
        thisProjectile.transform.localScale += new Vector3(projSize, projSize, projSize);
        Destroy(thisCharge);
        projSize = 0.0f;
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - animCut);
        if (baseScript.GetState() != CharacterBase.playerState.Dead && baseScript.GetState() != CharacterBase.playerState.TakingHit && baseScript.GetState() != CharacterBase.playerState.Stunned)
        {
            baseScript.SetState(CharacterBase.playerState.Idle);
        }
    }

    public void CancelAttack()
    {
        baseScript.SetState(CharacterBase.playerState.Idle);
        if (thisCharge != null)
        {
            Destroy(thisCharge);
        }
        projSize = 0.0f;
        isAiming = false;
        indicator.SetActive(false);
        indicator.transform.localScale = indicatorStartSize;
        anim.speed = 1f;
    }

    public void SetSpecialAttack(bool hasSpecial)
    {
        hasSpecialAttack = hasSpecial;
    }

    public bool GetSpecialAttack()
    {
        return hasSpecialAttack;
    }
}
