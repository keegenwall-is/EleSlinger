using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuseBoxManager : MinigameManager
{

    public List<Text> scores = new List<Text>();
    public GameObject[] items;

    private int[] playerScores = { -1, -1, -1, -1 };
    private GameObject[] itemSpawners;
    private bool[] itemSpawned = { false, false, false };
    private float[] spawnTimes = { 0, 0, 0 };
    private bool[] spawnerHasItem = { false, false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerNo; i++)
        {
            playerScores[i] = 0;
        }

        itemSpawners = GameObject.FindGameObjectsWithTag("Item Spawner");

        //Random a time in the first 6th of the game
        spawnTimes[0] = Random.Range(gameLengthStart * 5/6, gameLengthStart);
        //Random a time in the second 6th of the game
        spawnTimes[1] = Random.Range(gameLengthStart * 2/3, gameLengthStart * 5/6);
        //Random a time in the third 6th of the game, all items have spawned by half way through the game
        spawnTimes[2] = Random.Range(gameLengthStart * 1/2, gameLengthStart * 2/3);
    }

    protected override void OnTick()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (gameLength <= spawnTimes[i])
            {
                if (!itemSpawned[i])
                {
                    int randomSpawner = Random.Range(0, itemSpawners.Length);
                    //Stops two items from spawning at the same point
                    while (spawnerHasItem[randomSpawner])
                    {
                        randomSpawner = Random.Range(0, itemSpawners.Length);
                    }
                    Instantiate(items[i], itemSpawners[randomSpawner].transform.position, itemSpawners[randomSpawner].transform.rotation);
                    itemSpawned[i] = true;
                    spawnerHasItem[randomSpawner] = true;
                }
            }
        }
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        UpdateScore(player);
    }

    private void UpdateScore(GameObject player)
    {
        TakeHit takeHitScript = player.GetComponent<TakeHit>();
        if (takeHitScript != null)
        {
            //If player was not being attacked by anyone decrease their score by 1
            if (takeHitScript.GetAttacker() == null)
            {
                for (int i = 0; i < playerNo; i++)
                {
                    if (players[i] == player && playerScores[i] > 0)
                    {
                        playerScores[i]--;
                        scores[i].text = playerScores[i].ToString();
                    }
                }
            }
            else
            {
                //increase the score of the attacker
                for (int i = 0; i < playerNo; i++)
                {
                    if (players[i] == takeHitScript.GetAttacker())
                    {
                        playerScores[i]++;
                        scores[i].text = playerScores[i].ToString();
                    }
                }
            }
        }
    }
}
