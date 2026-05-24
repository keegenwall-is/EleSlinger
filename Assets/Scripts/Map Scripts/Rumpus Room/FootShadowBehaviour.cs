using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootShadowBehaviour : MonoBehaviour
{

    public Transform footTransform;
    public float height = 0.01f;

    private float xPos;
    private float zPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (footTransform != null)
        {
            xPos = footTransform.position.x;
            zPos = footTransform.position.z;
            transform.position = new Vector3(xPos, height, zPos);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
