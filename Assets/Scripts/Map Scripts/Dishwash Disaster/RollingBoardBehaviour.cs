using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBoardBehaviour : MonoBehaviour
{
    public float spawnCD;
    public GameObject rollingPin;

    private float currentSpawn;

    // Start is called before the first frame update
    void Start()
    {
        currentSpawn = spawnCD;
    }

    // Update is called once per frame
    void Update()
    {
        currentSpawn -= Time.deltaTime;
        if (currentSpawn <= 0)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y += 50f;
            spawnPos.z -= 10f;
            Instantiate(rollingPin, spawnPos, Quaternion.Euler(0, 0, 90));
            currentSpawn = spawnCD;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Contains("Rolling Pin"))
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.AddRelativeTorque(-Vector3.up * 100f, ForceMode.Impulse);
        }
    }
}
