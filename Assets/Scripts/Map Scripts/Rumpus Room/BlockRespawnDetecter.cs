using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRespawnDetecter : MonoBehaviour
{
    public int numOfBlocks = 6;
    public GameObject[] blocks;
    public float spawnHeight;
    public float respawnDur;

    private int numOfBlocksLeft;
    private bool respawning = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (respawning)
        {
            numOfBlocksLeft = 0;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Block")
        {
            numOfBlocksLeft++;
            //If all the blocks that originally spawned have left the respawn range then spawn new blocks
            if (numOfBlocksLeft >= numOfBlocks)
            {
                StartCoroutine(SpawnReplacementBlock());
            }
        }
    }

    private IEnumerator SpawnReplacementBlock()
    {
        respawning = true;
        yield return new WaitForSeconds(respawnDur);

        int randomDir = Random.Range(0, 4);
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += randomDir * 90;
        Quaternion spawnRot = Quaternion.Euler(rot);

        int randomPos = Random.Range(0, 3);
        Vector3 currentPos = transform.position;
        switch (randomPos)
        {
            case 0:
                currentPos.x = -25f;
                break;
            case 1:
                currentPos.x = 0f;
                break;
            case 2:
                currentPos.x = 25f;
                break;
        }
        transform.position = currentPos;

        Vector3 spawnPos = transform.position;
        spawnPos.y += spawnHeight;

        int randomSet = Random.Range(0, blocks.Length);

        Instantiate(blocks[randomSet], spawnPos, spawnRot);
        respawning = false;
    }
}
