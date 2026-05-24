using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowManBehaviour : MonoBehaviour
{
    public float pushMultiplier;
    public GameObject iceExplosion;
    public float chargeThreshold;

    private GameObject ice;
    private Rigidbody rb;
    private bool canCharge;

    // Start is called before the first frame update
    void Start()
    {
        ice = GameObject.FindGameObjectWithTag("Interactive Obj");
        rb = GetComponent<Rigidbody>();
        canCharge = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ice != null)
        {
            transform.LookAt(ice.transform);

            if (Vector3.Distance(transform.position, ice.transform.position) < chargeThreshold && canCharge)
            {
                rb.AddForce(transform.forward * pushMultiplier, ForceMode.Impulse);
            }
        }
        else
        {
            ice = GameObject.FindGameObjectWithTag("Interactive Obj");
        }
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name.Contains("Rink"))
        {
            Instantiate(iceExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (c.gameObject.name.Contains("Cube"))
        {
            canCharge = false;
            StartCoroutine(WaitUntilCanCharge());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Proj") || other.gameObject.name.Contains("Melee"))
        {
            AttackBase attackScript = other.gameObject.GetComponent<AttackBase>();

            rb.AddForce(attackScript.GetDirection(gameObject) * attackScript.GetPower() * pushMultiplier, ForceMode.Impulse);
        }
    }

    private IEnumerator WaitUntilCanCharge()
    {
        yield return new WaitForSeconds(2.0f);

        canCharge = true;
    }
}
