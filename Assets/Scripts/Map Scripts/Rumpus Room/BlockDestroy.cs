using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestroy : MonoBehaviour
{
    public float broomForce;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Broom")
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.mass = 1;
            rb.WakeUp();
            Vector3 forceDir = (other.transform.forward + transform.up).normalized;
            rb.AddForce(forceDir * broomForce, ForceMode.Impulse);
        }

        if (other.gameObject.name.Contains("Foot"))
        {
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.mass = 1;
                rb.WakeUp();
            }
        }
    }
}
