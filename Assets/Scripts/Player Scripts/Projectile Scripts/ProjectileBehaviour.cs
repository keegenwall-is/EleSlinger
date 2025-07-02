using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : AttackBase
{

    private Rigidbody rb;
    private ParticleSystem ps;

    public float speed;
    public float projTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        //Debug.DrawRay(transform.position, transform.forward * 2f, Color.red, 2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    protected override float SetDeleteTime()
    {
        return projTime;
    }

    protected override Vector3 GetDirection(GameObject enemy)
    {
        return transform.forward;
    }

    protected override float GetPower()
    {
        return transform.localScale.x;
    }

    protected override void DeleteEarly()
    {
        Destroy(gameObject);
    }
}
