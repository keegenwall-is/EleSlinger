using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    private CharacterBase baseScript;
    private Animator anim;
    private bool isAiming;

    public float animCut;
    public GameObject projectile;
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
        if (baseScript.thisController is Keyboard keyboard)
        {
            if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
            {
                if (keyboard.pKey.wasPressedThisFrame)
                {
                    baseScript.SetState(CharacterBase.playerState.Attacking);
                }
            }

            if (baseScript.GetState() == CharacterBase.playerState.Attacking)
            {
                if (keyboard.pKey.wasReleasedThisFrame)
                {
                    ResumeAnim();
                }
            }

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
    }

    public void EndAttack()
    {
        baseScript.SetState(CharacterBase.playerState.Idle);
    }
}
