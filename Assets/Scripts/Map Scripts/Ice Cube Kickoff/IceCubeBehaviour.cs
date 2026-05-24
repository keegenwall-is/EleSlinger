using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeBehaviour : MonoBehaviour
{

    public float minSize;
    public float pushMultiplier;
    public GameObject iceShadow;

    private Vector3 randomDir;
    private MinigameManager managerScript;
    private bool willShrink = true;
    private GameObject attachedPlayer;
    private Vector3 stuckPos;
    private Rigidbody rb;
    private GameObject thrower;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<MinigameManager>();

        randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        if (iceShadow != null)
        {
            GameObject thisIceShadow = Instantiate(iceShadow, transform.position, Quaternion.Euler(90f, 0f, 0f));
            FootShadowBehaviour iceShadowScript = thisIceShadow.GetComponent<FootShadowBehaviour>();
            iceShadowScript.footTransform = gameObject.transform;
            iceShadowScript.height = 20.01f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!willShrink)
        {
            stuckPos = transform.position;
            stuckPos.y = 20f;
            attachedPlayer.transform.position = stuckPos;

            attachedPlayer.transform.rotation = Quaternion.Euler(attachedPlayer.transform.eulerAngles.x, transform.eulerAngles.y, attachedPlayer.transform.eulerAngles.z);
        }
        else
        {
            rb.AddForce(Vector3.down * 1.5f, ForceMode.Acceleration);
        }
    }

    public void SetWillShrink(bool canShrink)
    {
        willShrink = canShrink;
        if (canShrink)
        {
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
        } 
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name.Contains("Goal"))
        {
            //Increase scores and spawn new ice cube
            managerScript.TriggerInteractiveObjectEvent(gameObject, thrower, c.gameObject);
            Destroy();
        }
    }

    private void Destroy()
    {
        if (thrower != null)
        {
            CharacterBase throwerBaseScript = thrower.GetComponent<CharacterBase>();
            GameObject thisKO = Instantiate(throwerBaseScript.KO, transform.position, transform.rotation);
        }

        if (gameObject.name.Contains("Pop"))
        {
            CharacterBase frozenBaseScript = null;

            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player"))
                {
                    frozenBaseScript = child.GetComponent<CharacterBase>();
                    break;
                }
            }
            frozenBaseScript.SetState(CharacterBase.playerState.Idle);
        }

        Destroy(gameObject);
    }
}
