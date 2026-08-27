using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageBehaviour : MonoBehaviour
{

    public int onFireCrates;
    public int onFireThreshold;
    public GameObject[] crateGroups;
    public GameObject explosion;
    public GameObject fireEffect;
    public float timeToExplosion;
    public int numOfFires;
    public float width;
    public float height;
    public float SpawnCaptureZoneCD;
    public List<GameObject> fires = new List<GameObject>();
    public GameObject captureZone;
    public float zoneRandomRange;

    private float spawnCaptureZoneCurrent;
    private float baseZoneCD;

    // Start is called before the first frame update
    void Start()
    {
        baseZoneCD = SpawnCaptureZoneCD;
        SpawnCaptureZoneCD += Random.Range(-zoneRandomRange, zoneRandomRange);
    }

    // Update is called once per frame
    void Update()
    {
        if (onFireCrates >= onFireThreshold)
        {
            StartCoroutine(CarriageExplosion());
        }

        spawnCaptureZoneCurrent += Time.deltaTime;

        if (spawnCaptureZoneCurrent >= SpawnCaptureZoneCD)
        {
            spawnCaptureZoneCurrent = 0f;
            SpawnCaptureZoneCD = baseZoneCD + Random.Range(-zoneRandomRange, zoneRandomRange);
            GameObject thisCaptureZone = Instantiate(captureZone, transform.position, Quaternion.identity);
            Vector3 spawnPos = transform.position;
            spawnPos.x += Random.Range(-33f, 33f);
            thisCaptureZone.transform.position = spawnPos;
            thisCaptureZone.transform.SetParent(transform);
        }
    }

    private IEnumerator CarriageExplosion()
    {
        onFireCrates = 0;

        for (int i = 0; i < numOfFires; i++)
        {
            GameObject thisFire = Instantiate(fireEffect);
            thisFire.transform.position = new Vector3(Random.Range(-width, width), 0f, Random.Range(-height, height)) + transform.position;
            thisFire.transform.SetParent(gameObject.transform, true);
            fires.Add(thisFire);
            yield return new WaitForSeconds(timeToExplosion / numOfFires);
        }

        yield return new WaitForSeconds(timeToExplosion);

        for (int i = fires.Count - 1; i >= 0; i--)
        {
            Destroy(fires[i]);
            fires.RemoveAt(i);
        }

        List<SphereCollider> explosionSCs = new List<SphereCollider>();

        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < crateGroups.Length; j++)
            {
                Vector3 spawnPos = crateGroups[j].transform.position;
                spawnPos.y += 5;
                GameObject thisExplosion = Instantiate(explosion, spawnPos, Quaternion.identity);
                explosionSCs.Add(thisExplosion.GetComponent<SphereCollider>());
            }
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.0f);

        foreach (SphereCollider sc in explosionSCs){
            sc.enabled = false;
        }
    }
}
