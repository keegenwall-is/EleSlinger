using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviour
{

    private CharacterBase baseScript;
    private GameObject thisMelee;

    public GameObject melee;
    public GameObject meleeSpawner;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
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
                if (keyboard.oKey.wasPressedThisFrame)
                {
                    baseScript.SetState(CharacterBase.playerState.Melee);
                    thisMelee = Instantiate(melee, meleeSpawner.transform);
                    MeleeBehaviour meleeScript = thisMelee.GetComponent<MeleeBehaviour>();
                    meleeScript.SetThrower(gameObject);
                    StartCoroutine(EndMelee());
                }
            }
        }
        else if (baseScript.thisController is Gamepad controller)
        {
            if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
            {
                if (controller.buttonNorth.wasPressedThisFrame)
                {
                    baseScript.SetState(CharacterBase.playerState.Melee);
                    thisMelee = Instantiate(melee, meleeSpawner.transform);
                    MeleeBehaviour meleeScript = thisMelee.GetComponent<MeleeBehaviour>();
                    meleeScript.SetThrower(gameObject);
                    StartCoroutine(EndMelee());
                }
            }
        }
    }

    private IEnumerator EndMelee()
    {
        yield return new WaitForSeconds(0.3f);
        if (baseScript.GetState() != CharacterBase.playerState.Dead)
        {
            baseScript.SetState(CharacterBase.playerState.Idle);
        }
    }
}
