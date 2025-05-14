using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    private Rigidbody rb;
    private ParticleSystem ps;
    private GameObject thrower;
    private GameObject thisHit;

    public float speed;
    public GameObject hit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        StartCoroutine(Delete());
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.red, 2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    public void SetThrower(GameObject thrower)
    {
        this.thrower = thrower;
    }

    IEnumerator Delete()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other != thrower)
        {
            thisHit = Instantiate(hit, transform.position, transform.rotation);
            HitBehaviour hitScript = thisHit.GetComponent<HitBehaviour>();
            hitScript.SetSize(transform.localScale);
            speed = 0.0f;
            Destroy(gameObject);
        }
    }
}
