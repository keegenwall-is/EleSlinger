using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenRackBehaviour : MonoBehaviour
{

    public GameObject stuckChar;
    public Transform[] charPoss;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform charPos in charPoss)
        {
            if (Random.Range(0, 4) == 0)
            {
                continue;
            }
            GameObject thisChar = Instantiate(stuckChar, transform);
            Vector3 spawnPos = charPos.position;
            spawnPos.x = Random.Range(spawnPos.x - 20, spawnPos.x + 20);
            thisChar.transform.position = spawnPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
