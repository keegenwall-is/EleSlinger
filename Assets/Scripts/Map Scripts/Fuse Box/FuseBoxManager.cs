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
    public GameObject electricPower;
    public float ePowerSpeedBuff;

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
        //Set state to attacking for the zoom in
        CharacterBase characterBase = player.GetComponent<CharacterBase>();
        characterBase.SetState(CharacterBase.playerState.Attacking);

        //Freeze the rest of the game
        Time.timeScale = 0.0f;

        Vector3 startPos = Camera.main.transform.position;

        CameraMovement camScript = Camera.main.GetComponent<CameraMovement>();
        camScript.enabled = false;

        float elapsed = 0f;
        float flashInterval = 0.25f;
        float flashDuration = 0.1f;
        float nextFlashTime = 0f;

        Vector3 targetPos = player.transform.position + player.transform.forward * 6.0f;
        targetPos.y += 8.0f;

        Vector3 lookAt = player.transform.position;
        lookAt.y += 4.0f;

        Camera.main.fieldOfView = 40f;
        characterBase.SetMaterial("Electric");

        //Moves the camera to look at the powered up characters face
        while (elapsed < 3.0f)
        {
            Camera.main.transform.LookAt(lookAt);
            Camera.main.transform.position = Vector3.Lerp(startPos, targetPos, elapsed);

            //toggles the material on and off
            if (elapsed >= nextFlashTime && elapsed < nextFlashTime + flashDuration)
            {
                characterBase.SetMaterial("Electric");
            }
            else
            {
                characterBase.SetMaterial("Material");
            }

            if (elapsed >= nextFlashTime + flashDuration)
            {
                nextFlashTime += flashInterval;
            }
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        characterBase.SetMaterial("Electric");

        //Camera returns to normal and game unpauses
        camScript.enabled = true;
        Time.timeScale = 1.0f;
        characterBase.SetState(CharacterBase.playerState.Idle);

        player.tag = "Immune";
        Vector3 spawnPos = player.transform.position;
        spawnPos.y += 4.0f;
        GameObject thisElectricPower = Instantiate(electricPower, spawnPos, player.transform.rotation, player.transform);

        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        playerMove.IncreaseSpeed(ePowerSpeedBuff);

        yield return new WaitForSeconds(electricPowerLength);

        RemoveItemsFromInventory(player);
        Destroy(thisElectricPower);
        playerMove.DecreaseSpeed(ePowerSpeedBuff);
        characterBase.SetMaterial("Material");
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
