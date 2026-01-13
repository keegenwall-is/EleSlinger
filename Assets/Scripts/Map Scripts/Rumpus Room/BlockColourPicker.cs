using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColourPicker : MonoBehaviour
{

    public Material[] colours;

    private List<GameObject> blocks = new List<GameObject>();
    private Material randMat;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            blocks.Add(child.gameObject);
        }

        randMat = colours[Random.Range(0, 4)];
        foreach (GameObject block in blocks)
        {
            MeshRenderer mr = block.GetComponent<MeshRenderer>();
            mr.material = randMat;
        }
    }
}
