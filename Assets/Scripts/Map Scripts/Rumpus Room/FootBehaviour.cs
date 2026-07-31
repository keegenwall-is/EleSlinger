using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBehaviour : MonoBehaviour
{

    public float rayDistance;
    public float searchSpeed;
    public float stompSpeed;
    public float liftSpeed;
    public float footHeight;
    public Transform shadowTransform;
    public int stunMashes;
    public float gracePeriod;
    public GameObject hit;
    public int changeAfterAttempts;

    private footState currentState;
    private GameController gameScript;
    private List<GameObject> players = new List<GameObject>();
    private Rigidbody rb;
    private Vector3 moveDir = new Vector3(0, 0, 0);
    private GameObject closestPlayer;
    private GameObject lastStomped;
    private GameObject lastClosest;
    private int stompAttempts;
    private float sphereCastRadius = 0.1f;
    private Coroutine stompedCoroutine;

    public enum footState
    {
        Searching,
        Stomping,
        Lifting
    }

    // Start is called before the first frame update
    void Start()
    {
        gameScript = GameObject.FindGameObjectWithTag("Game Controller").GetComponent<GameController>();
        if (gameScript)
        {
            players = gameScript.GetPlayers();
        }
        rb = GetComponent<Rigidbody>();
        SetState(footState.Searching);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == footState.Searching)
        {
            //Detect players to see when to stomp
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, sphereCastRadius, -transform.forward, out hit, rayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (hit.collider.gameObject != lastStomped)
                    {
                        SetState(footState.Stomping);
                    }
                }
            }
            Debug.DrawRay(transform.position, -transform.forward * rayDistance, Color.red);

            float minDist = 354f; /*Hypotenuse of the floor boards plane ie. max distance*/
            closestPlayer = null;
            for (int i = 0; i < players.Count; i++)
            {

                if (players[i] == lastStomped)
                {
                    continue;
                }

                float thisDist = Vector3.Distance(transform.position, players[i].transform.position);
                if (thisDist < minDist)
                {
                    minDist = thisDist;
                    closestPlayer = players[i];
                }
            }

            if (lastClosest != closestPlayer)
            {
                lastClosest = closestPlayer;
                stompAttempts = 0;
            }
        }
        else if (currentState == footState.Stomping)
        {
            rb.velocity = -transform.forward * stompSpeed;
        }
        else if (currentState == footState.Lifting)
        {
            rb.velocity = transform.forward * liftSpeed;

            if (transform.position.y > footHeight)
            {
                SetState(footState.Searching);
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentState == footState.Searching)
        {
            if (closestPlayer != null)
            {
                moveDir = closestPlayer.transform.position - transform.position;
                moveDir.y = 0f;
                rb.velocity = moveDir.normalized * searchSpeed;
            }
            else
            {
                if (rb)
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }
    }


    public void SetState(footState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStateEnter(newState);
        }
    }

    public void OnStateEnter(footState state)
    {
        switch (state)
        {
            case footState.Searching:
                break;
            case footState.Stomping:
                stompAttempts++;
                if (stompAttempts >= changeAfterAttempts)
                {
                    lastStomped = closestPlayer;
                    if (stompedCoroutine != null)
                    {
                        StopCoroutine(stompedCoroutine);
                    }
                    stompedCoroutine = StartCoroutine(NoLastStomped());
                }
                break;
            case footState.Lifting:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStunned stunnedScript = collision.gameObject.GetComponent<PlayerStunned>();
            stunnedScript.SetMashes(stunMashes);
            stunnedScript.Stunned();
            lastStomped = closestPlayer;
            if (stompedCoroutine != null)
            {
                StopCoroutine(stompedCoroutine);
            }
            stompedCoroutine = StartCoroutine(NoLastStomped());
        }

        if (currentState == footState.Stomping)
        {
            Instantiate(hit, transform.position, Quaternion.identity);
            SetState(footState.Lifting);
        }
    }

    private IEnumerator NoLastStomped()
    {
        yield return new WaitForSeconds(gracePeriod);

        lastStomped = null;
    }
}
