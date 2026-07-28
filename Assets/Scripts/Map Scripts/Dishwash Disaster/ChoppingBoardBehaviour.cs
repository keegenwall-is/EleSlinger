using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoardBehaviour : MonoBehaviour
{

    public GameObject knife;
    public float dropKnivesCD;
    public int numOfKnives;
    public float width;
    public float height;

    private float dropKnivesCurrent;
    private float sectionWidth;

    // Start is called before the first frame update
    void Start()
    {
        dropKnivesCurrent = dropKnivesCD;
        sectionWidth = width / numOfKnives;
    }

    // Update is called once per frame
    void Update()
    {
        dropKnivesCurrent -= Time.deltaTime;

        if (dropKnivesCurrent <= 0)
        {
            dropKnivesCurrent = dropKnivesCD;
            StartCoroutine(SpawnKnives());
        }
    }
    
    private IEnumerator SpawnKnives()
    {
        for (int i = 0; i < numOfKnives; i++)
        {
            float minX = -(width / 2) + (i * sectionWidth);
            float maxX = minX + sectionWidth;

            float randX = Random.Range(minX, maxX) + transform.position.x;
            float randZ = Random.Range(-(height / 2), height / 2) + transform.position.z - 5f;
            Vector3 spawnPos = new Vector3(randX, 50f, randZ);
            Instantiate(knife, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
