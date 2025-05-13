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
    private float chargeSize;

    public float animCut;
    public GameObject projectile;
    public GameObject charge;
    public GameObject magicSpawner;
    public float aimSpeed;
    public float chargeSpeed;

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
                Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
                if (moveZ != 0f || moveX != 0f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, aimSpeed * Time.deltaTime);
                }
                chargeSize += 1.5f * Time.deltaTime;
                thisCharge.transform.localScale += new Vector3(chargeSpeed, chargeSpeed, chargeSpeed) * Time.deltaTime;
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
            }
        }
    }

    public void ResumeAnim()
    {
        anim.speed = 1f;
        isAiming = false;
    }

    public void SpawnProjectile()
    {
        thisProjectile = Instantiate(projectile, magicSpawner.transform.position, transform.rotation);
        ProjectileBehaviour projScript = thisProjectile.GetComponent<ProjectileBehaviour>();
        projScript.SetThrower(gameObject);
        thisProjectile.transform.localScale = thisProjectile.transform.localScale + new Vector3(chargeSize, chargeSize, chargeSize);
        Destroy(thisCharge);
        chargeSize = 0.0f;
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - animCut);
        baseScript.SetState(CharacterBase.playerState.Idle);
    }
}
