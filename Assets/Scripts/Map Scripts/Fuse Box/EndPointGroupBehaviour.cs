using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointGroupBehaviour : MonoBehaviour
{

    public GameObject[] endPoints;
    public int survivorCount;

    // Start is called before the first frame update
    void Awake()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < endPoints.Length; i++)
        {
            availableIndices.Add(i);
        }

        HashSet<int> survivorIndices = new HashSet<int>();
        for (int i = 0; i < survivorCount; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            survivorIndices.Add(availableIndices[randomIndex]);
            availableIndices.RemoveAt(randomIndex);
        }

        for (int i = 0; i < endPoints.Length; i++)
        {
            if (survivorIndices.Contains(i))
            {
                continue;
            }

            DestroyImmediate(endPoints[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
