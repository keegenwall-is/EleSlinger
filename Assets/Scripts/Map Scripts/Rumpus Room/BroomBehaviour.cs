using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BroomBehaviour : MonoBehaviour
{

    public float rayDistance;
    public float foundDur;
    public float broomSpeed;
    public float searchSpeed;
    public float minBroomPosX;
    public float maxBroomPosX;
    public float minBroomPosZ;
    public float maxBroomPosZ;
    public Image indicator;
    public float speedIncrement;

    private broomState currentState;
    private Rigidbody rb;
    private bool movingLeft = true;
    private bool movingUp = true;

    public enum broomState
    {
        Searching,
        Found,
        Sweeping
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetState(broomState.Searching);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState == broomState.Searching)
        {
            //Detect players to see when to sweep
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    SetState(broomState.Found);
                }
            }
            Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);

            //handle searching movement
            if (movingUp)
            {
                rb.velocity = transform.right * searchSpeed;
            }
            else
            {
                rb.velocity = -transform.right * searchSpeed;
            }

            //if movingLeft is false this means that the the broom has been rotated so that moving movingUp is actually movingDown, thus the
            //movingUp check needs to be swapped around
            if (movingLeft)
            {
                if ((transform.position.z > maxBroomPosZ && movingUp) || (transform.position.z < minBroomPosZ && !movingUp))
                {
                    movingUp = !movingUp;
                }
            }
            else
            {
                if ((transform.position.z > maxBroomPosZ && !movingUp) || (transform.position.z < minBroomPosZ && movingUp))
                {
                    movingUp = !movingUp;
                }
            }

        }
        else if (currentState == broomState.Sweeping)
        {
            //sweep from right to left or left to right
            rb.velocity = transform.forward * broomSpeed;

            if ((transform.position.x < minBroomPosX && movingLeft) || (transform.position.x > maxBroomPosX && !movingLeft))
            {
                movingLeft = !movingLeft;
                transform.forward = -transform.forward;
                SetState(broomState.Searching);
            }
        }
    }

    public void SetState(broomState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStateEnter(newState);
        }
    }

    public void OnStateEnter(broomState state)
    {
        switch (state)
        {
            case broomState.Searching:
                indicator.color = new Color(1, 1, 0);
                break;
            case broomState.Found:
                indicator.color = new Color(1, 0, 0);
                StartCoroutine(SweepAlert());
                break;
            case broomState.Sweeping:
                broomSpeed += speedIncrement;
                //Speed capped because if broom is too fast ray cast wont capture it.
                if (searchSpeed <= 175)
                {
                    searchSpeed += speedIncrement;
                }
                break;
        }
    }

    private IEnumerator SweepAlert()
    {
        yield return null;

        rb.velocity = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(foundDur);

        movingUp = !movingUp;
        SetState(broomState.Sweeping);
    }
}