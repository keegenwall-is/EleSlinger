using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopFrostStabilizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.eulerAngles = Vector3.zero;
    }
}
