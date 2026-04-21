using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStunned : MonoBehaviour
{
    public Animator mashAnim;
    public GameObject stunStars;

    private CharacterBase baseScript;
    private PlayerAttack PAscript;
    private Rigidbody rb;
    private int maxMashes;
    private int currentMashes;
    private GameObject theseStars;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
        PAscript = GetComponent<PlayerAttack>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.GetState() == CharacterBase.playerState.Stunned)
        {
            if (baseScript.thisController is Keyboard keyboard)
            {
                if (keyboard.spaceKey.wasPressedThisFrame)
                {
                    CheckMashes();
                }
            }
            else if (baseScript.thisController is Gamepad controller)
            {
                if (controller.buttonSouth.wasPressedThisFrame)
                {
                    CheckMashes();
                }
            }
        }
    }

    public void SetMashes(int mashes)
    {
        maxMashes = mashes;
    }

    public void Stunned()
    {
        rb.velocity = Vector3.zero;
        currentMashes = 0;

        if (baseScript.GetState() == CharacterBase.playerState.Attacking)
        {
            PAscript.CancelAttack();
        }

        Vector3 spawnPos = transform.position;
        spawnPos.y += 4f;
        Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);

        if (theseStars == null)
        {
            theseStars = Instantiate(stunStars, spawnPos, spawnRot);
        }
        
        baseScript.SetState(CharacterBase.playerState.Stunned);
    }

    private void CheckMashes()
    {
        currentMashes++;
        mashAnim.Play("MashInstruction");
        if (currentMashes >= maxMashes)
        {
            baseScript.SetState(CharacterBase.playerState.Idle);
            currentMashes = 0;
            Destroy(theseStars);
        }
    }
}
