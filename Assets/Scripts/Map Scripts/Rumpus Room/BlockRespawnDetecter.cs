using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRespawnDetecter : MonoBehaviour
{
    public int numOfBlocks = 6;
    public GameObject blocks;
    public float spawnHeight;
    public float respawnDur;

    private int numOfBlocksLeft;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Block")
        {
            numOfBlocksLeft++;
            //If all the blocks that originally spawned have left the respawn range then spawn new blocks
            if (numOfBlocksLeft >= numOfBlocks)
            {
                numOfBlocksLeft = 0;
                Vector3 spawnPos = transform.position;
                spawnPos.y += spawnHeight;
                StartCoroutine(SpawnReplacementBlock(spawnPos));
            }
        }
    }

    private IEnumerator SpawnReplacementBlock(Vector3 spawnPos)
    {
        yield return new WaitForSeconds(respawnDur);

        Instantiate(blocks, spawnPos, transform.rotation);
    }
}
