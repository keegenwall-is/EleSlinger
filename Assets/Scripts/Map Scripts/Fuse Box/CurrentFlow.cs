using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentFlow : MonoBehaviour
{

    public GameObject[] currents;

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
        if (other.CompareTag("Current"))
        {
            foreach (GameObject current in currents)
            {
                current.SetActive(true);
            }
        }
    }
}
