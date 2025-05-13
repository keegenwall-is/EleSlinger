using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    private CharacterBase baseScript;
    private Animator anim;
    private bool isAiming;
    private GameObject thisCharge;

    public float animCut;
    public GameObject projectile;
    public GameObject charge;
    public GameObject magicSpawner;
    public float aimSpeed;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
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
                if (keyboard.aKey.isPressed)
                {
                    transform.Rotate(0, aimSpeed * -1, 0);
                } 
                else if (keyboard.dKey.isPressed)
                {
                    transform.Rotate(0, aimSpeed, 0);
                }
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
        Instantiate(projectile, magicSpawner.transform.position, transform.rotation);
        Destroy(thisCharge);
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - animCut);
        baseScript.SetState(CharacterBase.playerState.Idle);
    }
}
