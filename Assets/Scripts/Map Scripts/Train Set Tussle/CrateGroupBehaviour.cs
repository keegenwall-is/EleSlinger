using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateGroupBehaviour : MonoBehaviour
{

    public float spawnRange;
    public GameObject crate;

    private float spawnHeight = 2.5f;
    private Vector3[] spawnPoss;

    // Start is called before the first frame update
    void Start()
    {
        SpawnCrates();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCrates()
    {
        spawnPoss = new Vector3[] {
            new Vector3(spawnRange, spawnHeight, 0f),
            new Vector3(-spawnRange, spawnHeight, 0f),
            new Vector3(Random.Range(-spawnRange, spawnRange), spawnHeight, spawnRange),
            new Vector3(Random.Range(-spawnRange, spawnRange), spawnHeight, -spawnRange)
        };

        for (int i = 0; i < spawnPoss.Length; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                continue;
            }

            GameObject thisCrate = Instantiate(crate, gameObject.transform);
            thisCrate.transform.position = spawnPoss[i] + transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Carriage Explosion"))
        {
            SpawnCrates();
        }
    }
}
