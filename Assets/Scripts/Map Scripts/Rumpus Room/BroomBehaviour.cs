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
    public Image[] indicators;
    public float speedIncrement;
    public PCGRumpusRoom pcgScript;
    public float[] possibleSweeps = new float[4];
    public float restingTime = 10f;
    public GameObject fire;
    public float fireWidth;
    public float fireZ;

    private broomState currentState;
    private Rigidbody rb;
    private bool movingLeft = true;
    private bool movingUp = true;
    private float zSweep = 0f;
    private int zSweepIndex;
    private int lastSweep;
    private bool resting = false;
    private float fireCurrent;
    private List<GameObject> fires = new List<GameObject>();
    private float fireCD;

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
        zSweepIndex = Random.Range(0, possibleSweeps.Length);
        zSweep = possibleSweeps[zSweepIndex];
        SetState(broomState.Searching);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState == broomState.Searching)
        {
            //handle searching movement
            if (transform.position.z <= zSweep + 1f && transform.position.z >= zSweep - 1f && !resting)
            {
                SetState(broomState.Found);
            }

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

            fireCurrent += Time.deltaTime;

            if (fireCurrent >= fireCD)
            {
                fireCurrent = 0f;
                Vector3 spawnPos1 = transform.position;
                Vector3 spawnPos2 = spawnPos1;

                spawnPos1.z += fireZ;
                spawnPos2.z -= fireZ;

                GameObject thisFire1 = Instantiate(fire, spawnPos1, Quaternion.identity);
                GameObject thisFire2 = Instantiate(fire, spawnPos2, Quaternion.identity);

                fires.Add(thisFire1);
                fires.Add(thisFire2);
            }

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

                foreach (GameObject fire in fires)
                {
                    Destroy(fire);
                }
                fires.Clear();

                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                switch (zSweepIndex)
                {
                    case 0:
                        pcgScript.RegenerateRow(1);
                        pcgScript.RegenerateRow(2);
                        break;
                    case 1:
                        pcgScript.RegenerateRow(2);
                        pcgScript.RegenerateRow(3);
                        break;
                    case 2:
                        pcgScript.RegenerateRow(3);
                        pcgScript.RegenerateRow(4);
                        break;
                    case 3:
                        pcgScript.RegenerateRow(3);
                        pcgScript.RegenerateRow(4);
                        break;
                }
                zSweepIndex = Random.Range(0, possibleSweeps.Length);
                //to stop the broom sweeping the same spot twice in a row
                /*while (zSweepIndex == lastSweep)
                {
                    zSweepIndex = Random.Range(0, possibleSweeps.Length);
                }
                lastSweep = zSweepIndex;*/
                zSweep = possibleSweeps[zSweepIndex];
                HandleIndicator(false);
                StartCoroutine(Resting());
                break;
            case broomState.Found:
                HandleIndicator(true);
                StartCoroutine(SweepAlert());
                break;
            case broomState.Sweeping:
                rb.constraints |= RigidbodyConstraints.FreezePositionZ;
                broomSpeed += speedIncrement;
                fireCD = fireWidth / broomSpeed;
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

    private void HandleIndicator(bool danger)
    {
        int i = 0;
        foreach (Image indicator in indicators)
        {
            if (i == 1)
            {
                if (danger)
                {
                    indicator.color = new Color(1, 0.5f, 0, 0.3f);
                }
                else
                {
                    indicator.color = new Color(1, 1, 0, 0.3f);
                }
            }
            else
            {
                if (danger)
                {
                    indicator.color = new Color(1, 0.5f, 0);
                }
                else
                {
                    indicator.color = new Color(1, 1, 0);
                }
            }
            i++;
        }
    }

    private IEnumerator Resting()
    {
        resting = true;
        yield return new WaitForSeconds(restingTime);
        restingTime *= 9f / 10f;
        resting = false;
    }
}