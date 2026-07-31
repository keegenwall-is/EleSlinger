using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviour
{

    public GameObject melee;
    public GameObject meleeSpawner;
    public float meleeCD = 0.5f;

    private CharacterBase baseScript;
    private GameObject thisMelee;
    private float meleeCurrent;
    private bool inputBuffer;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
    }

    // Update is called once per frame
    void Update()
    {
        meleeCurrent -= Time.deltaTime;
        if (meleeCurrent <= 0)
        {
            meleeCurrent = 0;
            //Checks the kind of controller the player is using
            if (baseScript.thisController is Keyboard keyboard)
            {
                //Starts the attack if the player is idle or running and presses the attack button
                if (keyboard.oKey.wasPressedThisFrame)
                {
                    if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
                    {
                        Melee();
                    }
                    else if (baseScript.GetState() == CharacterBase.playerState.Dashing)
                    {
                        inputBuffer = true;
                    }
                }
                
            }
            else if (baseScript.thisController is Gamepad controller)
            {
                if (controller.buttonNorth.wasPressedThisFrame)
                {
                    if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
                    {
                        Melee();
                    }
                    else if (baseScript.GetState() == CharacterBase.playerState.Dashing)
                    {
                        inputBuffer = true;
                    }
                }
            }

            if (inputBuffer)
            {
                if ((baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running) && meleeCurrent <= 0)
                {
                    inputBuffer = false;
                    Melee();
                }
            }
        }
        
    }

    private void Melee()
    {
        baseScript.SetState(CharacterBase.playerState.Melee);
        thisMelee = Instantiate(melee, meleeSpawner.transform);
        MeleeBehaviour meleeScript = thisMelee.GetComponent<MeleeBehaviour>();
        meleeScript.SetThrower(gameObject);
        StartCoroutine(EndMelee());
        meleeCurrent = meleeCD;
    }

    private IEnumerator EndMelee()
    {
        yield return new WaitForSeconds(0.3f);
        //if (baseScript.GetState() != CharacterBase.playerState.Dead && baseScript.GetState() != CharacterBase.playerState.TakingHit && baseScript.GetState() != CharacterBase.playerState.Stunned)
        //{
        //    baseScript.SetState(CharacterBase.playerState.Idle);
        //}
        if (baseScript.GetState() == CharacterBase.playerState.Melee)
        {
            baseScript.SetState(CharacterBase.playerState.Idle);
        }
    }
}
