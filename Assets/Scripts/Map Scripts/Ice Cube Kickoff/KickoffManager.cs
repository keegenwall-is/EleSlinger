using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KickoffManager : MinigameManager
{

    [Header("Kickoff Variables")]
    public List<Text> scores = new List<Text>();
    public List<GameObject> goals = new List<GameObject>();
    public GameObject[] iceSpawners;
    public GameObject[] popsicleSpawners;
    public GameObject iceCube;
    public GameObject popsicle;
    public GameObject popsicleCube;
    public GameObject popsicleFrost;
    public float iceSpawnCD;
    public float popsicleSpawnCD;
    public int iceMashes;
    public GameObject iceNova;
    public GameObject[] playerSpawners; 

    private int[] playerScores = { 0, 0 };
    private float iceSpawnCurrent;
    private float popsicleSpawnCurrent;
    private int randIceSpawn;
    private int randPopSpawn;
    private float winningScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (playerNo <= 2)
        {
            Vector3 spawnPos = playerSpawners[0].transform.position;
            spawnPos.z = 0;
            playerSpawners[0].transform.position = spawnPos;
        }

        if (playerNo <= 3)
        {
            Vector3 spawnPos = playerSpawners[1].transform.position;
            spawnPos.z = 0;
            playerSpawners[1].transform.position = spawnPos;
        }
    }

    protected override void OnTick()
    {
        iceSpawnCurrent += 1 * Time.deltaTime;
        popsicleSpawnCurrent += 1 * Time.deltaTime;

        if (iceSpawnCurrent >= iceSpawnCD)
        {
            iceSpawnCurrent = 0;
            randIceSpawn = Random.Range(0, iceSpawners.Length);
            Instantiate(iceCube, iceSpawners[randIceSpawn].transform);
        }

        if (popsicleSpawnCurrent >= popsicleSpawnCD)
        {
            popsicleSpawnCurrent = 0;
            randPopSpawn = Random.Range(0, 4);
            Instantiate(popsicle, popsicleSpawners[randPopSpawn].transform);
        }

        if (overTime)
        {
            OnMinigameEnd();
        }
    }

    protected override void OnInteractiveObjectEvent(GameObject obj, GameObject player, GameObject other)
    {
        //increase score for player who shot the goal and decrease for the goal scored against

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i] == other)
            {
                if (i == 1)
                {
                    playerScores[0]++;
                    scores[0].text = playerScores[0].ToString();
                    StartCoroutine(ScoreAnimation(true, players[0]));
                }
                else
                {
                    playerScores[1]++;
                    scores[1].text = playerScores[1].ToString();
                    StartCoroutine(ScoreAnimation(true, players[1]));
                }
            }
        }

        randIceSpawn = Random.Range(0, iceSpawners.Length);
        Instantiate(iceCube, iceSpawners[randIceSpawn].transform);
        iceSpawnCurrent = 0;
    }

    public override void HandleItemPickup(GameObject item, GameObject actor)
    {
        PlayerAttack attackScript = actor.GetComponent<PlayerAttack>();
        attackScript.SetSpecialAttack(true);

        Vector3 spawnPos = actor.transform.position;
        spawnPos.y += 4.0f;
        Instantiate(popsicleFrost, spawnPos, actor.transform.rotation, actor.transform);
    }

    public override void HandleSpecialAttack(GameObject hitPlayer, GameObject thrower)
    {
        GameObject nova = Instantiate(iceNova, hitPlayer.transform.position, Quaternion.identity);
        nova.GetComponent<IceNovaBehaviour>().SetThrower(thrower);
        StartCoroutine(DestroyAfterTime(nova));
    }

    private IEnumerator DestroyAfterTime(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(obj);
    }

    protected override void OnMinigameEnd()
    {
        if (overTime)
        {
            //As soon as a player beats the winning score or only 1 player is left with the winning score, the game ends
            int maxScoreCounter = 0;
            for (int i = 0; i < playerScores.Length; i++)
            {
                if (playerScores[i] == winningScore + 1)
                {
                    overTime = false;
                    gameController.IncreaseRoundWins(players[i]);
                    return;
                }
                else if (playerScores[i] == winningScore)
                {
                    maxScoreCounter++;
                }
            }

            if (maxScoreCounter == 1)
            {
                for (int i = 0; i < playerScores.Length; i++)
                {
                    if (playerScores[i] == winningScore)
                    {
                        overTime = false;
                        gameController.IncreaseRoundWins(players[i]);
                    }
                }
            }
        }
        else
        {
            //Check for biggest score
            for (int i = 0; i < playerScores.Length; i++)
            {
                if (playerScores[i] > winningScore)
                {
                    winningScore = playerScores[i];
                }
            }

            //if more than one player has the winning score then go into overtime
            int maxScoreCounter = 0;
            for (int i = 0; i < playerScores.Length; i++)
            {
                if (playerScores[i] == winningScore)
                {
                    maxScoreCounter++;
                }
                if (maxScoreCounter > 1)
                {
                    overTime = true;
                    countdown.text = "OVERTIME";
                    countdown.color = Color.red;
                    break;
                }
            }

            if (!overTime)
            {
                if (playerScores[0] == winningScore)
                {
                    gameController.IncreaseRoundWins(players[0]);
                    gameController.IncreaseRoundWins(players[2]);
                }
                else
                {
                    gameController.IncreaseRoundWins(players[1]);
                    gameController.IncreaseRoundWins(players[3]);
                }
            }
        }
    }
}
