using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehaviour : MonoBehaviour
{
    public float pushMultiplier;
    public GameObject fire;

    private Rigidbody rb;
    private bool isOnFire;
    private CarriageBehaviour carriageScript;
    private bool immune;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(IsImmune());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (isOnFire)
        //{
        //    return;
        //}
        if (other.gameObject.name.Contains("Platform"))
        {
            carriageScript = other.gameObject.GetComponent<CarriageBehaviour>();
            return;
        }

        if (immune)
        {
            return;
        }

        //to make the crates move from melees and projectiles
        /*if (other.gameObject.name.Contains("Proj") || other.gameObject.name.Contains("Melee"))
        {
            AttackBase attackScript = other.gameObject.GetComponent<AttackBase>();

            rb.AddForce(attackScript.GetDirection(gameObject) * attackScript.GetPower() * pushMultiplier, ForceMode.Impulse);
        }
        else*/
        if (other.gameObject.name.Contains("Bullet"))
        {
            if (!isOnFire && carriageScript != null)
            {
                Instantiate(fire, transform);
                carriageScript.onFireCrates++;
            }
            isOnFire = true;
        }
        else if (other.gameObject.name.Contains("Carriage Explosion"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Platform"))
        {
            if (isOnFire && carriageScript != null)
            {
                carriageScript.onFireCrates--;
            }
        }
    }

    private IEnumerator IsImmune()
    {
        immune = true;
        yield return new WaitForSeconds(3.0f);
        immune = false;
    }
}
