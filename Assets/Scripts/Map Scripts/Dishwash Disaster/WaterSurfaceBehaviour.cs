using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurfaceBehaviour : MonoBehaviour
{

    public GameObject splash;
    public float splashHeight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("ObImmune"))
        {
            Vector3 spawnPos = other.transform.position;
            spawnPos.y = splashHeight;
            Instantiate(splash, other.transform.position, Quaternion.identity);
        }
    }
}
