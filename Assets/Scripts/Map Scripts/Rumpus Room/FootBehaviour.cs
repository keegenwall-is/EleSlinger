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

    private footState currentState;
    private GameController gameScript;
    private List<GameObject> players = new List<GameObject>();
    private Rigidbody rb;
    private Vector3 moveDir = new Vector3(0, 0, 0);
    private GameObject closestPlayer;

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
        players = gameScript.GetPlayers();
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
            if (Physics.Raycast(transform.position, -transform.up, out hit, rayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    SetState(footState.Stomping);
                }
            }
            Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.red);

            float minDist = 354f; /*Hypotenuse of the floor boards plane ie. max distance*/
            for (int i = 0; i < players.Count; i++)
            {
                float thisDist = Vector3.Distance(transform.position, players[i].transform.position);
                if (thisDist < minDist)
                {
                    minDist = thisDist;
                    closestPlayer = players[i];
                }
            }

            if (closestPlayer != null)
            {
                moveDir = closestPlayer.transform.position - transform.position;
                moveDir.y = 0f;
                rb.velocity = moveDir.normalized * searchSpeed;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        else if (currentState == footState.Stomping)
        {
            rb.velocity = -transform.up * stompSpeed;
        }
        else if (currentState == footState.Lifting)
        {
            rb.velocity = transform.up * liftSpeed;

            if (transform.position.y > footHeight)
            {
                SetState(footState.Searching);
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
                break;
            case footState.Lifting:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerStunned stunnedScript = collision.gameObject.GetComponent<PlayerStunned>();
            stunnedScript.SetMashes(stunMashes);
            stunnedScript.Stunned();
        }

        if (currentState == footState.Stomping)
        {
            SetState(footState.Lifting);
        }
    }
}
