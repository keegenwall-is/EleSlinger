using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{

    public float fallSpeed = 5;
    public bool fallDown = false;

    private CharacterBase baseScript;
    private PlayerMove moveScript;
    private Rigidbody rb;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
        moveScript = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody>();
        fallDown = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (baseScript.GetState() == CharacterBase.playerState.Falling)
        {
            fallSpeed += 0.5f;

            if (!fallDown)
            {
                velocity = transform.forward * moveScript.moveSpeed;
            }
            else
            {
                velocity = -transform.up * moveScript.moveSpeed;
            }

            velocity.y = -fallSpeed;

            rb.velocity = velocity;
        }

        //Returning the fallSpeed back to original, should not be hard coded
        if (fallSpeed > 0 && baseScript.GetState() != CharacterBase.playerState.Falling)
        {
            fallSpeed = 5f;
            fallDown = false;
        }
    }
}
