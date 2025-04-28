using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    private CharacterBase baseScript;

    public float animCut;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.thisController is Keyboard keyboard)
        {
            if (keyboard.qKey.wasPressedThisFrame && baseScript.GetState() != CharacterBase.playerState.Dashing)
            {
                StartCoroutine(Attack()); 
            }
        }
    }

    IEnumerator Attack()
    {
        baseScript.SetState(CharacterBase.playerState.Attacking);

        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(baseScript.anim.GetCurrentAnimatorStateInfo(0).length - animCut);

        baseScript.SetState(CharacterBase.playerState.Idle);
    }
}
