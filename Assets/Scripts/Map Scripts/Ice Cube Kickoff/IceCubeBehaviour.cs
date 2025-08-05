using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeBehaviour : MonoBehaviour
{

    Rigidbody rb;
    private GameObject thrower;
    public float shrinkSpeed;
    public float minSize;
    public float driftStrength;
    public float driftPause;

    private Vector3 randomDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        //StartCoroutine(DriftAfterTime());
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale -= Vector3.one * Time.deltaTime * shrinkSpeed;

        if  (transform.localScale.x <= minSize)
        {
            Destroy(gameObject);
        }

        //if (rb.velocity.magnitude <= driftStrength)
        //{
        //    StartCoroutine(DriftAfterTime());
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Proj") || other.gameObject.name.Contains("Melee"))
        {
            AttackBase attackScript = other.gameObject.GetComponent<AttackBase>();
            thrower = attackScript.GetThrower();

            rb.AddForce(attackScript.GetDirection(gameObject) * attackScript.GetPower(), ForceMode.Impulse);
        }
    }

    private IEnumerator DriftAfterTime()
    {
        yield return new WaitForSeconds(driftPause);
        //Vector3 currentVelo = rb.velocity;
        rb.velocity = randomDir * driftStrength;
    }
}
