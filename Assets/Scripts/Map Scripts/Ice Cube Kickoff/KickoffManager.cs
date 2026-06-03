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
    public GameObject popsicleFrost;
    public float popsicleSpawnCD;
    public float snowManSpawnCD;
    public int iceMashes;
    public GameObject iceNova;
    public GameObject[] playerSpawners;
    public GameObject mainCam;
    public GameObject startIce;
    public GameObject snowMan;
    public float randSpawnAmount;

    private int[] playerScores = { 0, 0 };
    private float popsicleSpawnCurrent;
    private float snowManSpawnCurrent;
    private int randIceSpawn;
    private int randPopSpawn;
    private float winningScore = 0;
    private CameraMovement camMoveScript;
    private float randPopSpawnCD;
    private float randSnowManSpawnCD;

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

        camMoveScript = mainCam.GetComponent<CameraMovement>();
        randPopSpawnCD = Random.Range(popsicleSpawnCD - randSpawnAmount, popsicleSpawnCD + randSpawnAmount);
        randSnowManSpawnCD = Random.Range(snowManSpawnCD - randSpawnAmount, snowManSpawnCD + randSpawnAmount);
    }

    protected override void OnAllReady()
    {
        camMoveScript.FindObject(startIce);
    }

    protected override void OnTick()
    {
        popsicleSpawnCurrent += 1 * Time.deltaTime;
        snowManSpawnCurrent += 1 * Time.deltaTime;

        if (popsicleSpawnCurrent >= randPopSpawnCD)
        {
            popsicleSpawnCurrent = 0;
            randPopSpawn = Random.Range(0, 4);
            Instantiate(popsicle, popsicleSpawners[randPopSpawn].transform);
            randPopSpawnCD = Random.Range(popsicleSpawnCD - randSpawnAmount, popsicleSpawnCD + randSpawnAmount);
        }

        if (snowManSpawnCurrent >= randSnowManSpawnCD)
        {
            snowManSpawnCurrent = 0;
            Vector3 spawnPos = new Vector3(Random.Range(-30f, 30f), 50f, Random.Range(-30f, 30f));
            Instantiate(snowMan, spawnPos, Quaternion.identity);
            randSnowManSpawnCD = Random.Range(snowManSpawnCD - randSpawnAmount, snowManSpawnCD + randSpawnAmount);
        }

        if (overTime)
        {
            OnMinigameEnd();
        }
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        GameObject spawn = SetPlayerSpawn(player);
        KillPlayer(player, spawn);
    }

    protected override void OnInteractiveObjectEvent(GameObject obj, GameObject player, GameObject other)
    {
        //increase score for player who shot the goal and decrease for the goal scored against
        camMoveScript.ForgetObject(obj);
        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i] == other)
            {
                if (i == 1)
                {
                    playerScores[0]++;
                    scores[0].text = playerScores[0].ToString();
                    StartCoroutine(ScoreAnimation(true, players[0]));
                    if (playerNo >= 3)
                    {
                        Instantiate(scoreEffect, players[2].transform.position, Quaternion.Euler(-90f, 0f, 0f));
                    }
                }
                else
                {
                    playerScores[1]++;
                    scores[1].text = playerScores[1].ToString();
                    StartCoroutine(ScoreAnimation(true, players[1]));
                    if (playerNo >= 4)
                    {
                        Instantiate(scoreEffect, players[3].transform.position, Quaternion.Euler(-90f, 0f, 0f));
                    }
                }
            }
        }

        randIceSpawn = Random.Range(0, iceSpawners.Length);

        if (!obj.name.Contains("Pop"))
        {
            StartCoroutine(SpawnAfterTime());
        }
    }

    public override void HandleItemPickup(GameObject item, GameObject actor)
    {
        PlayerAttack attackScript = actor.GetComponent<PlayerAttack>();
        attackScript.SetSpecialAttack(true);

        Vector3 spawnPos = actor.transform.position;
        spawnPos.y += 2.0f;
        GameObject thisPopFrost = Instantiate(popsicleFrost, spawnPos, actor.transform.rotation, actor.transform);
        thisPopFrost.transform.localScale /= actor.transform.localScale.x;
    }

    public override void HandleSpecialAttack(GameObject hitPlayer, GameObject thrower)
    {
        Vector3 spawnPos = hitPlayer.transform.position;
        spawnPos.y += 2.0f;
        GameObject nova = Instantiate(iceNova, spawnPos, Quaternion.identity);
        nova.GetComponent<IceNovaBehaviour>().SetThrower(thrower);
        StartCoroutine(DestroyAfterTime(nova));
    }

    private IEnumerator DestroyAfterTime(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(obj);
    }

    private IEnumerator SpawnAfterTime()
    {
        yield return new WaitForSeconds(2.0f);

        GameObject thisIce = Instantiate(iceCube, iceSpawners[randIceSpawn].transform);
        camMoveScript.FindObject(thisIce);
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
                    if (playerNo >= 3)
                    {
                        gameController.IncreaseRoundWins(players[2]);
                    }
                }
                else
                {
                    gameController.IncreaseRoundWins(players[1]);
                    if (playerNo >= 4)
                    {
                        gameController.IncreaseRoundWins(players[3]);
                    }
                }
            }
        }
    }
}
