using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBallBehaviour : MonoBehaviour
{

    public GameObject fireBall;
    public float pushMultiplier;
    public float fireExtinguishThreshold;
    public float maxHeight;

    private Rigidbody rb;
    private GameObject thrower;
    private bool checkVelo = false;
    private GameObject thisFireBall;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (thisFireBall != null)
        {
            thisFireBall.transform.rotation = Quaternion.identity;
        }*/

        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }

        if (transform.position.y > maxHeight)
        {
            Vector3 pos = transform.position;
            pos.y = maxHeight;
            transform.position = pos;
        }

        if (checkVelo)
        {
            if (rb.velocity.magnitude < fireExtinguishThreshold)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                checkVelo = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Proj") || other.gameObject.name.Contains("Melee"))
        {
            AttackBase attackScript = other.gameObject.GetComponent<AttackBase>();
            thrower = attackScript.GetThrower();

            rb.AddForce(attackScript.GetDirection(gameObject) * attackScript.GetPower() * pushMultiplier, ForceMode.Impulse);

            if (transform.childCount == 0)
            {
                StartCoroutine(SetFireAfterTime());
                StartCoroutine(CheckVeloAfterTime());
            }
        }
        else if (other.gameObject.name.Contains("Broom") || other.gameObject.name.Contains("Car"))
        {
            if (transform.childCount == 0)
            {
                StartCoroutine(SetFireAfterTime());
                StartCoroutine(CheckVeloAfterTime());
            }
        }
        else if (other.gameObject.name.Contains("SafeZone"))
        {
            rb.velocity = -rb.velocity;
        }

    }

    private IEnumerator SetFireAfterTime()
    {
        yield return new WaitForSeconds(0.1f);

        thisFireBall = Instantiate(fireBall, gameObject.transform);
    }

    private IEnumerator CheckVeloAfterTime()
    {
        yield return new WaitForSeconds(1.0f);

        checkVelo = true;
    }
}
