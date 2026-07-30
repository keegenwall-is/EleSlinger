using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RumpusRoomManager : MinigameManager
{
    public List<Text> scoresTxts = new List<Text>();
    public GameObject broom;
    public int punishment;
    public GameObject horizontalBar;
    public float multiplierDuration;
    public GameObject multiplierVFX;
    public GameObject multiplier;
    public float multiplierCD;
    public GameObject multiplierReadyCanvas;

    private int[] playerScores = { -1, -1, -1, -1 };
    private float winningScore = 0;
    private bool[] hasMultiplier = { false, false, false, false };
    private float multiplierCurrent;
    private bool multiplierSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerNo; i++)
        {
            playerScores[i] = 0;
        }

        if (playerNo == 2)
        {
            Destroy(horizontalBar);
        }
    }

    protected override void OnAllReady()
    {
        broom.SetActive(true);
    }

    protected override void OnTick()
    {
        if (!multiplierSpawned)
        {
            multiplierCurrent += Time.deltaTime;
        }

        if (multiplierCurrent >= multiplierCD)
        {
            multiplierCurrent = 0f;
            multiplierSpawned = true;
            Vector3 spawnPos = new Vector3(0f, 3.5f, 0f);
            Instantiate(multiplier, spawnPos, Quaternion.identity);
            multiplierReadyCanvas.SetActive(true);
            StartCoroutine(TurnOffCanvas());
        }

        if (overTime)
        {
            OnMinigameEnd();
        }
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                GameObject spawn = SetPlayerSpawn(player);
                KillPlayer(player, spawn);

                playerScores[i] -= punishment;
                if (playerScores[i] < 0)
                {
                    playerScores[i] = 0;
                }
                scoresTxts[i].text = playerScores[i].ToString();
                StartCoroutine(ScoreAnimation(false, player, punishment));
            }
        }
    }

    public override void HandleItemPickup(GameObject item, GameObject actor)
    {
        if (item.name.Contains("Coin"))
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == actor)
                {
                    if (hasMultiplier[i])
                    {
                        playerScores[i] += 2;
                        StartCoroutine(ScoreAnimation(true, actor, 2));
                    }
                    else
                    {
                        playerScores[i]++;
                        StartCoroutine(ScoreAnimation(true, actor));
                    }

                    scoresTxts[i].text = playerScores[i].ToString();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == actor)
                {
                    PlayerMove moveScript = actor.GetComponent<PlayerMove>();
                    Vector3 spawnPos = actor.transform.position;
                    spawnPos.y += 2.0f;
                    GameObject thisMultiplierVFX = Instantiate(multiplierVFX, spawnPos, actor.transform.rotation, actor.transform);
                    thisMultiplierVFX.transform.localScale /= actor.transform.localScale.x;
                    hasMultiplier[i] = true;
                    StartCoroutine(EndMultiplier(i, thisMultiplierVFX, moveScript));
                    multiplierSpawned = false;
                    moveScript.IncreaseSpeed(1.5f);
                }
            }
        }

    }

    private IEnumerator EndMultiplier(int index, GameObject thisMultiplierVFX, PlayerMove moveScript)
    {
        yield return new WaitForSeconds(multiplierDuration);

        hasMultiplier[index] = false;
        Destroy(thisMultiplierVFX);
        moveScript.DecreaseSpeed();
    }

    public IEnumerator TurnOffCanvas()
    {
        yield return new WaitForSeconds(3.0f);

        multiplierReadyCanvas.SetActive(false);
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
