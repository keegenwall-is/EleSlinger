using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehaviour : MonoBehaviour
{

    public float knifeSpeed;
    public Transform[] magicTrans;
    public GameObject magic;
    public GameObject warning;
    public float warningSpawnHeight;

    private Vector3 origPos;
    private DishwashManager managerScript;
    private bool markForDestruction = false;
    private GameObject thisWarning;

    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.position;
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<DishwashManager>();
        Vector3 warningPos = transform.position;
        warningPos.y = warningSpawnHeight;
        thisWarning = Instantiate(warning, warningPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        origPos.x -= managerScript.plateSpeed * Time.deltaTime;

        if (thisWarning)
        {
            Vector3 warningPos = origPos;
            warningPos.y = warningSpawnHeight;
            warningPos.z += 5f;
            warningPos.x += 12.5f;
            thisWarning.transform.position = warningPos;
        }

        if (transform.position.y > 4f)
        {
            origPos.y -= knifeSpeed * Time.deltaTime;
        }
        else
        {
            if (!markForDestruction)
            {
                StartCoroutine(DestroyAfterTime());
                markForDestruction = true;
            }
        }

        transform.position = origPos;
    }

    private IEnumerator DestroyAfterTime()
    {
        Destroy(thisWarning);

        yield return new WaitForSeconds(6.0f);

        foreach (Transform magicSpawn in magicTrans)
        {
            Instantiate(magic, magicSpawn.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
