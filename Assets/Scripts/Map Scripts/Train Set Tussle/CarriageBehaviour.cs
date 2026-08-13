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

    public List<GameObject> fires = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (onFireCrates >= onFireThreshold)
        {
            StartCoroutine(CarriageExplosion());
        }
    }

    private IEnumerator CarriageExplosion()
    {
        onFireCrates = 0;

        for (int i = 0; i < numOfFires; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-width, width), 0f, Random.Range(-height, height)) + transform.position;
            GameObject thisFire = Instantiate(fireEffect, spawnPos, Quaternion.identity);
            fires.Add(thisFire);
            yield return new WaitForSeconds(timeToExplosion / numOfFires);
        }

        yield return new WaitForSeconds(timeToExplosion);

        for (int i = fires.Count - 1; i >= 0; i--)
        {
            Destroy(fires[i]);
            fires.RemoveAt(i);
        }

        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < crateGroups.Length; j++)
            {
                Vector3 spawnPos = crateGroups[j].transform.position;
                spawnPos.y += 5;
                Instantiate(explosion, spawnPos, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
