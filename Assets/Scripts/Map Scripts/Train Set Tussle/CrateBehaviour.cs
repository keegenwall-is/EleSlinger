using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehaviour : MonoBehaviour
{
    public float pushMultiplier;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Proj") || other.gameObject.name.Contains("Melee"))
        {
            AttackBase attackScript = other.gameObject.GetComponent<AttackBase>();

            rb.AddForce(attackScript.GetDirection(gameObject) * attackScript.GetPower() * pushMultiplier, ForceMode.Impulse);
        }
        else if (other.gameObject.name.Contains("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
