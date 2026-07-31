using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellBehaviour : MonoBehaviour
{

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
        if (other.gameObject.name.Contains("Broom") || other.gameObject.name.Contains("X2") || other.gameObject.name.Contains("StartSafeZone"))
        {
            Destroy(gameObject);
        }
    }
}
