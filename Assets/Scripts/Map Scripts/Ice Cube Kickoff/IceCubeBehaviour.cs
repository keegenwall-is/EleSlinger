using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeBehaviour : MonoBehaviour
{

    public float shrinkSpeed;
    public float minSize;
    public float driftStrength;
    public float driftPause;
    public float destroyTime;
    public float pushMultiplier;
    public Transform frostTrans;

    private Vector3 randomDir;
    private MinigameManager managerScript;
    private bool willShrink = true;
    private GameObject attachedPlayer;
    private Vector3 stuckPos;
    private Rigidbody rb;
    private GameObject thrower;
    private Quaternion origPopRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<MinigameManager>();

        randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        origPopRot = frostTrans.localRotation;
        //StartCoroutine(DriftAfterTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (willShrink)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * shrinkSpeed;

            if (transform.localScale.x <= minSize)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            stuckPos = transform.position;
            stuckPos.y = 0f;
            attachedPlayer.transform.position = stuckPos;

            attachedPlayer.transform.rotation = Quaternion.Euler(attachedPlayer.transform.eulerAngles.x, transform.eulerAngles.y, attachedPlayer.transform.eulerAngles.z);
        }

        //if (rb.velocity.magnitude <= driftStrength)
        //{
        //    StartCoroutine(DriftAfterTime());
        //}
    }

    void LateUpdate()
    {
        frostTrans.eulerAngles = Vector3.zero;
    }

    public void SetWillShrink(bool canShrink)
    {
        willShrink = canShrink;
        if (canShrink)
        {
            print("constraints changed");
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
    }

    public void SetAttachedPlayer(GameObject player)
    {
        attachedPlayer = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Proj") || other.gameObject.name.Contains("Melee"))
        {
            AttackBase attackScript = other.gameObject.GetComponent<AttackBase>();
            thrower = attackScript.GetThrower();

            rb.AddForce(attackScript.GetDirection(gameObject) * attackScript.GetPower() * pushMultiplier, ForceMode.Impulse);

            Vector3 randomTorqueDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
            ).normalized;

            float torqueAmount = attackScript.GetPower() * 0.5f;
            rb.AddTorque(randomTorqueDirection * torqueAmount, ForceMode.Impulse);
        } else if (other.gameObject.name.Contains("Goal"))
        {
            //Increase and decrease scores and spawn new ice cube
            managerScript.TriggerInteractiveObjectEvent(gameObject, thrower, other.gameObject);
            StartCoroutine(DestroyAfterTime());
        }
    }

    private IEnumerator DriftAfterTime()
    {
        yield return new WaitForSeconds(driftPause);
        //Vector3 currentVelo = rb.velocity;
        rb.velocity = randomDir * driftStrength;
    }

    private IEnumerator DestroyAfterTime()
    {
        if (thrower != null)
        {
            CharacterBase throwerBaseScript = thrower.GetComponent<CharacterBase>();
            GameObject thisKO = Instantiate(throwerBaseScript.KO, transform.position, transform.rotation);
        }

        yield return new WaitForSeconds(destroyTime);

        if (gameObject.name.Contains("Pop"))
        {
            CharacterBase frozenBaseScript = transform.GetChild(1).gameObject.GetComponent<CharacterBase>();
            frozenBaseScript.SetState(CharacterBase.playerState.Idle);
        }

        Destroy(gameObject);
    }
}
