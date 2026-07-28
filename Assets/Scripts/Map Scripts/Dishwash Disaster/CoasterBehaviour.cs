using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterBehaviour : MonoBehaviour
{
    public float swapCoasterCD;
    public GameObject coaster;
    public float spawnAngle;
    public float radius;
    public int numOfCoasters;
    public GameObject magic;

    private float swapCoasterCurrent = 0f;
    private Vector3 spawnPos;
    private float currentAngle;
    private List<GameObject> coasters = new List<GameObject>();
    private DishwashManager managerScript;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<DishwashManager>();
        for (int i = 0; i < numOfCoasters; i++)
        {
            SpawnCoaster();
        }
    }

    // Update is called once per frame
    void Update()
    {
        swapCoasterCurrent += Time.deltaTime;

        if (swapCoasterCurrent >= swapCoasterCD)
        {
            swapCoasterCurrent = 0f;
            SpawnCoaster();

            StartCoroutine(SinkAndDestroy(coasters[0]));
            coasters.RemoveAt(0);
        }

        foreach (GameObject coaster in coasters)
        {
            coaster.transform.position -= Vector3.right * managerScript.plateSpeed * Time.deltaTime;
        }
    }
    
    private void SpawnCoaster()
    {
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 localOffset = new Vector3(Mathf.Sin(radians) * radius, 0.5f, Mathf.Cos(radians) * radius);
        spawnPos = transform.position + localOffset;
        GameObject thisMagic = Instantiate(magic, spawnPos, Quaternion.identity);
        thisMagic.transform.localScale *= 2;
        GameObject newCoaster = Instantiate(coaster, spawnPos, Quaternion.identity);
        coasters.Add(newCoaster);
        currentAngle += spawnAngle;
    }

    private IEnumerator SinkAndDestroy(GameObject thisCoaster)
    {
        Vector3 startPos = thisCoaster.transform.position;
        Vector3 endPos = startPos - new Vector3(0, 50f, 0);

        float elapsedTime = 0f;

        while (elapsedTime < 1.0f)
        {
            float progress = elapsedTime / 1.0f;

            thisCoaster.transform.position = Vector3.Lerp(startPos, endPos, progress);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(thisCoaster);
    }
}
