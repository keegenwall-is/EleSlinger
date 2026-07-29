using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowManBehaviour : MonoBehaviour
{
    public float pushMultiplier;
    public GameObject iceExplosion;
    public float chargeThreshold;
    public float rotSpeed = 5f;

    private GameObject ice;
    private Rigidbody rb;
    private bool canCharge;
    public KickoffManager managerScript;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<KickoffManager>();
        FindClosestCube();
        rb = GetComponent<Rigidbody>();
        canCharge = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ice != null)
        {
            Vector3 dir = ice.transform.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime * 100f);

            if (Vector3.Distance(transform.position, ice.transform.position) < chargeThreshold && canCharge)
            {
                rb.AddForce(transform.forward * pushMultiplier, ForceMode.Impulse);
            }
        }

        FindClosestCube();
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

    public void FindClosestCube()
    {
        float minDistance = float.MaxValue;
        GameObject closestCube = null;
        foreach (GameObject cube in managerScript.iceCubes)
        {
            float dist = Vector3.Distance(transform.position, cube.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestCube = cube;
            }
        }

        ice = closestCube;
    }
}
