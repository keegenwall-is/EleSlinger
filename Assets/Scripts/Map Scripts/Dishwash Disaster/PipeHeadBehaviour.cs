using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeHeadBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 2) > 0)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
