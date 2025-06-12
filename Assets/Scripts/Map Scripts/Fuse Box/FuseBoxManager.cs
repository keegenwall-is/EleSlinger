using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuseBoxManager : MinigameManager
{

    public List<Text> scores = new List<Text>();
    public GameObject[] items;
    public List<Image> p1ItemIcons = new List<Image>();
    public List<Image> p2ItemIcons = new List<Image>();
    public List<Image> p3ItemIcons = new List<Image>();
    public List<Image> p4ItemIcons = new List<Image>();
    public Sprite[] hasItemIcons;
    public List<Sprite> p1NoItemIcons = new List<Sprite>();
    public List<Sprite> p2NoItemIcons = new List<Sprite>();
    public List<Sprite> p3NoItemIcons = new List<Sprite>();
    public List<Sprite> p4NoItemIcons = new List<Sprite>();
    public float minItemRespawnTime;
    public float maxItemRespawnTime;
    public float electricPowerLength;

    private int[] playerScores = { -1, -1, -1, -1 };
    private GameObject[] itemSpawners;
    private bool[] itemSpawned = { false, false, false };
    private float[] spawnTimes = { 0, 0, 0 };
    private string[] spawnerHasItem = { "none", "none", "none", "none", "none" };
    private float[] numOfItems = { -1, -1, -1, -1 };
    private List<List<Image>> itemIcons = new List<List<Image>>();
    private List<List<Sprite>> baseItemIcons = new List<List<Sprite>>();
    private bool[,] playerInventories = { { false, false, false }, { false, false, false }, { false, false, false }, { false, false, false } };

    // Start is called before the first frame update
    void Start()
    {
        //set numOfItems and player scores to 0 for active players
        for (int i = 0; i < playerNo; i++)
        {
            playerScores[i] = 0;
            numOfItems[i] = 0;
        }

        //Fill 2D Lists of Images and Sprites
        itemIcons.Add(p1ItemIcons);
        itemIcons.Add(p2ItemIcons);
        itemIcons.Add(p3ItemIcons);
        itemIcons.Add(p4ItemIcons);
        baseItemIcons.Add(p1NoItemIcons);
        baseItemIcons.Add(p2NoItemIcons);
        baseItemIcons.Add(p3NoItemIcons);
        baseItemIcons.Add(p4NoItemIcons);

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
        //Spawns items as the timer goes down
        for (int i = 0; i < items.Length; i++)
        {
            if (gameLength <= spawnTimes[i])
            {
                if (!itemSpawned[i])
                {
                    int randomSpawner = Random.Range(0, itemSpawners.Length);
                    //Stops two items from spawning at the same point
                    while (spawnerHasItem[randomSpawner] != "none")
                    {
                        randomSpawner = Random.Range(0, itemSpawners.Length);
                    }
                    Instantiate(items[i], itemSpawners[randomSpawner].transform.position, itemSpawners[randomSpawner].transform.rotation);
                    itemSpawned[i] = true;
                    spawnerHasItem[randomSpawner] = items[i].tag;
                }
            }
        }
    }

    public override void HandleItemPickup(GameObject pickedupItem, GameObject playerWithItem)
    {
        //Change the UI for the player picking up the item to show the correct item in their inventory
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == playerWithItem)
            {
                for (int j = 0; j < items.Length; j++)
                {
                    if (items[j].tag == pickedupItem.tag)
                    {
                        itemIcons[i][j].sprite = hasItemIcons[j];
                        playerInventories[i, j] = true;

                        //When item is picked up, allow new items to spawn on this spawner
                        for (int k = 0; k < spawnerHasItem.Length; k++)
                        {
                            if (spawnerHasItem[k] == items[j].tag)
                            {
                                spawnerHasItem[k] = "none";
                            }
                        }

                        bool allItems = true;

                        for (int k = 0; k < items.Length; k++)
                        {
                            if (playerInventories[i, k] == false)
                            {
                                allItems = false;
                            }
                        }

                        if (allItems)
                        {
                            StartCoroutine(ElectricPower(players[i]));
                        }
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator ElectricPower(GameObject player)
    {
        player.tag = "Immune";
        //Instantiate();
        yield return new WaitForSeconds(electricPowerLength);

        RemoveItemsFromInventory(player);
        player.tag = "Player";
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        UpdateScore(player);

        RemoveItemsFromInventory(player);
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

    private void RemoveItemsFromInventory(GameObject player)
    {
        //Remove items from inventory
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                for (int j = 0; j < items.Length; j++)
                {
                    if (playerInventories[i, j] == true)
                    {
                        itemIcons[i][j].sprite = baseItemIcons[i][j];
                        playerInventories[i, j] = false;
                        //Allow items to be respawned
                        float newSpawnTime = Random.Range(minItemRespawnTime, maxItemRespawnTime);
                        spawnTimes[j] = gameLength - newSpawnTime;
                        itemSpawned[j] = false;
                    }
                }
                break;
            }
        }
    }
}
