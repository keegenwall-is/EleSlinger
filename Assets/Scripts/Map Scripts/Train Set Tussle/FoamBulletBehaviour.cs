using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamBulletBehaviour : MonoBehaviour
{

    public float bulletSpeed;
    public GameObject explosion;
    public GameObject target;
    public bool isHoming;
    public float explodeAfter = 10f;

    private Vector3 dir;
    private TrainSetManager managerScript;
    private CharacterBase baseScript;
    private bool useHoming;
    private float explodeAfterCurrent = 0f;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<TrainSetManager>();
        FindTarget();

        if (isHoming)
        {
            baseScript = target.GetComponent<CharacterBase>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);

        explodeAfterCurrent += Time.deltaTime;

        if (explodeAfterCurrent >= explodeAfter)
        {
            GameObject thisExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            managerScript.environmentObjects.Add(thisExplosion);
            StartCoroutine(managerScript.RemoveFromEnvironment(thisExplosion, 0.4f));
            Destroy(gameObject);
        }

        if (isHoming)
        {
            if (baseScript.GetState() == CharacterBase.playerState.Dead || baseScript.GetState() == CharacterBase.playerState.Out)
            {
                useHoming = false;
            }
            else
            {
                useHoming = true;
            }
        }


        if (useHoming)
        {
            FindTarget();
        }
    }

    private void FindTarget()
    {
        Vector3 targetPos = target.transform.position;
        targetPos.y += 3f;
        dir = targetPos - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = lookRot * Quaternion.Euler(90, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!other.gameObject.name.Contains("Melee"))
        //{
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            GameObject thisExplosion = Instantiate(explosion, contactPoint, Quaternion.identity);
            managerScript.environmentObjects.Add(thisExplosion);
            StartCoroutine(managerScript.RemoveFromEnvironment(thisExplosion, 0.4f));
            Destroy(gameObject);
            //managerScript.environmentObjects.Remove(gameObject);
        //}
    }
}
