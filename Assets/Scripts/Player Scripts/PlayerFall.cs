using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{

    private CharacterBase baseScript;
    private PlayerMove moveScript;
    private Rigidbody rb;
    public float fallSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
        moveScript = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (baseScript.GetState() == CharacterBase.playerState.Falling)
        {
            fallSpeed += 0.5f;

            Vector3 velocity = transform.forward * moveScript.moveSpeed;
            velocity.y = -fallSpeed;

            rb.velocity = velocity;
        }

        if (fallSpeed > 0 && baseScript.GetState() != CharacterBase.playerState.Falling)
        {
            fallSpeed = 5f;
        }
    }
}
