using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainSetManager : MinigameManager
{
    [Header("Train Set Variables")]
    public GameObject blockGroup;
    public float environmentSpawnCD = 1f;
    public float environmentSpeed;
    public GameObject rail;
    public float bulletSpawnCD = 1f;
    public GameObject[] bullets;
    public List<Text> scoresTxts = new List<Text>();

    private float environmentSpawnCurrent;
    public List<GameObject> environmentObjects = new List<GameObject>();
    private float railSpawnCD;
    private float railSpawnCurrent;
    private float railLength = 29.5f;
    private float bulletSpawnCurrent;
    private int numOut;
    private CameraMovement camMoveScript;
    private int[] playerScores = { -1, -1, -1, -1 };
    private float winningScore = 0;
    private bool roundEnded = false;
    private float roundCD = 3f;
    private float roundCurrent;

    // Start is called before the first frame update
    void Start()
    {
        camMoveScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        bulletSpawnCD /= playerNo;
        railSpawnCD = railLength / environmentSpeed;

        for (float i = 200f; i >= -200f; i -= railLength)
        {
            Vector3 spawnPos = new Vector3(i, -10f, 0f);
            GameObject thisRail = Instantiate(rail, spawnPos, Quaternion.identity);
            environmentObjects.Add(thisRail);
        }

        for (int i = 0; i < playerNo; i++)
        {
            playerScores[i] = 0;
        }
    }

    protected override void OnTick()
    {
        environmentSpawnCurrent += Time.deltaTime;

        if (environmentSpawnCurrent >= environmentSpawnCD)
        {
            environmentSpawnCurrent = 0f;
            int updown = Random.Range(0, 2);
            float randomZ = 0f;
            if (updown == 0)
            {
                randomZ = Random.Range(17, 80);
            }
            else
            {
                randomZ = Random.Range(-17, -80);
            }
            Vector3 spawnPos = new Vector3(200f, -10f, randomZ);
            GameObject thisEnvObj = Instantiate(blockGroup, spawnPos, Quaternion.identity);
            environmentObjects.Add(thisEnvObj);
        }

        railSpawnCurrent += Time.deltaTime;

        if (railSpawnCurrent >= railSpawnCD)
        {
            railSpawnCurrent = 0f;
            railSpawnCD = railLength / environmentSpeed;
            Vector3 spawnPos = new Vector3(200f, -10f, 0f);
            GameObject thisRail = Instantiate(rail, spawnPos, Quaternion.identity);
            environmentObjects.Add(thisRail);
        }

        bulletSpawnCurrent += Time.deltaTime;

        if (bulletSpawnCurrent >= bulletSpawnCD)
        {
            int updown = Random.Range(0, 2);
            float randomZ = 0f;
            if (updown == 0)
            {
                randomZ = 200f;
            }
            else
            {
                randomZ = -200f;
            }

            bulletSpawnCurrent = 0f;
            float randomX = Random.Range(-100f, 150f);
            Vector3 spawnPos = new Vector3(randomX, 3f, randomZ);
            int randBullet = Random.Range(0, 6);
            int bulletNo = 0;
            if (randBullet == 0)
            {
                bulletNo = 1;
            }
            GameObject thisBullet = Instantiate(bullets[bulletNo], spawnPos, Quaternion.identity);
            FoamBulletBehaviour bulletScript = thisBullet.GetComponent<FoamBulletBehaviour>();
            int randomTarget = Random.Range(0, players.Count);
            CharacterBase baseScript = players[randomTarget].GetComponent<CharacterBase>();
            while (baseScript.GetState() == CharacterBase.playerState.Out)
            {
                randomTarget = Random.Range(0, players.Count);
                baseScript = players[randomTarget].GetComponent<CharacterBase>();
            }
            bulletScript.target = players[randomTarget];
            //environmentObjects.Add(thisBullet);
        }

        for (int i = environmentObjects.Count - 1; i >= 0; i--)
        {
            if (environmentObjects[i] == null)
            {
                continue;
            }

            environmentObjects[i].transform.Translate(-Vector3.right * environmentSpeed * Time.deltaTime);

            if (environmentObjects[i].transform.position.x <= -200)
            {
                Destroy(environmentObjects[i]);
                environmentObjects.RemoveAt(i);
            }
        }

        if (numOut >= playerNo - 1)
        {
            roundEnded = true;
            for (int i = 0; i < players.Count; i++)
            {
                CharacterBase baseScript = players[i].GetComponent<CharacterBase>();
                if (baseScript.GetState() == CharacterBase.playerState.Out)
                {
                    GameObject spawn = SetPlayerSpawn(players[i]);
                    KillPlayer(players[i], spawn);
                    camMoveScript.FindPlayers();
                }
                else if (numOut > 0)
                {
                    playerScores[i]++;
                    scoresTxts[i].text = playerScores[i].ToString();
                    StartCoroutine(ScoreAnimation(true, players[i]));
                }
            }
            numOut = 0;
        }

        if (roundEnded)
        {
            roundCurrent += Time.deltaTime;

            if (roundCurrent >= roundCD)
            {
                roundCurrent = 0;
                roundEnded = false;
            }
        }

        if (overTime)
        {
            OnMinigameEnd();
        }
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        if (roundEnded)
        {
            GameObject spawn = SetPlayerSpawn(player);
            KillPlayer(player, spawn);
        }
        else
        {
            CharacterBase baseScript = player.GetComponent<CharacterBase>();
            baseScript.SetState(CharacterBase.playerState.Out);
            numOut++;
        }
    }

    public IEnumerator RemoveFromEnvironment(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        environmentObjects.Remove(obj);
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
                for (int i = 0; i < playerScores.Length; i++)
                {
                    if (playerScores[i] == winningScore)
                    {
                        gameController.IncreaseRoundWins(players[i]);
                    }
                }
            }
        }
    }
}
