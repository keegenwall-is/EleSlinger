using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarBehaviour : MonoBehaviour
{
    public float speed;
    public float bumpSpeed;
    public float rayDistance = 10f;
    public GameObject warning;
    public GameObject explosion;
    public AudioClip honkSFX;
    public AudioClip thumpSFX;

    private Rigidbody rb;
    private bool bump = false;
    private RectTransform rectTransform;
    private AudioSource ap;
    private bool hasHonked;
    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ap = GetComponent<AudioSource>();
        renderer = GetComponentInChildren<Renderer>();
        rectTransform = warning.GetComponent<RectTransform>();

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
    }

    void Update()
    {
        RaycastHit hit;

        bool findPlayer = Physics.SphereCast(transform.position, 4f, transform.forward, out hit, rayDistance);

        if (findPlayer && hit.collider.CompareTag("Player")) {
            warning.SetActive(true);
            if (!hasHonked && !bump)
            {
                ap.volume = 0.5f;
                ap.PlayOneShot(honkSFX);
                hasHonked = true;
            }
            Vector3 newPos = rectTransform.localPosition;

            newPos.z = hit.distance - 5.0f;
            rectTransform.localPosition = newPos;
        }
        else
        {
            warning.SetActive(false);
            hasHonked = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!bump)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            rb.linearVelocity = -transform.forward * bumpSpeed;
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.name.Contains("Block") || collider.gameObject.name.Contains("Ball") || collider.gameObject.name.Contains("Stuffy") || collider.gameObject.name.Contains("Car"))
        {
            if (!bump)
            {
                bump = true;
                StartCoroutine(Bump());
            }
        }
    }

    private void OnBecameVisible()
    {
        Debug.Log(gameObject.name + " is now visible to a camera.");
    }

    private void OnBecameInvisible()
    {
        Debug.Log(gameObject.name + " is no longer visible to any camera.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Broom"))
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (other.gameObject.name.Contains("SafeZone"))
        {
            if (!bump)
            {
                bump = true;
                StartCoroutine(Bump());
            }
        }
    }

    private IEnumerator Bump()
    {
        if (ap != null && renderer.isVisible)
        {
            ap.volume = 0.2f;
            ap.PlayOneShot(thumpSFX);
        }

        yield return new WaitForSeconds(1.0f);

        float origBumpSpeed = bumpSpeed;
        bumpSpeed = 0;

        RaycastHit hitLeft;
        RaycastHit hitRight;

        // Cast ray to the left
        bool leftHit = Physics.SphereCast(transform.position, 4f, -transform.right, out hitLeft, rayDistance);

        // Cast ray to the right
        bool rightHit = Physics.SphereCast(transform.position, 4f, transform.right, out hitRight, rayDistance);

        float leftDistance = leftHit ? hitLeft.distance : 0f;
        float rightDistance = rightHit ? hitRight.distance : 0f;

        float turnSpeed = 360f;
        Quaternion targetRotation = Quaternion.identity;


        if (leftDistance < 9.0f && rightDistance < 9.0f)
        {
            targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        }
        else
        {
            if (leftHit || rightHit)
            {
                if (leftDistance > rightDistance)
                {
                    targetRotation = Quaternion.LookRotation(-transform.right, Vector3.up);
                }
                else if (rightDistance > leftDistance)
                {
                    targetRotation = Quaternion.LookRotation(transform.right, Vector3.up);
                }
                else
                {
                    targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
                }
            }
        }

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );

            yield return null; // wait one frame
        }

        bump = false;
        bumpSpeed = origBumpSpeed;
    }
}
