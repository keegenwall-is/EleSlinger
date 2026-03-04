using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RumpusRoomManager : MinigameManager
{
    //public int maxLives;
    //public List<Text> livesTxts = new List<Text>();
    public List<Text> scoresTxts = new List<Text>();
    public GameObject broom;
    public int punishment;
    //public GameObject[] cameras;

    //private int[] lives = { 0, 0, 0, 0 };
    private int[] playerScores = { -1, -1, -1, -1 };
    private float winningScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerNo; i++)
        {
            playerScores[i] = 0;
        }
    }

    protected override void OnAllReady()
    {
        broom.SetActive(true);
    }

    protected override void OnTick()
    {
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
                StartCoroutine(ScoreAnimation(false, player));
            }
        }
    }

    public override void HandleItemPickup(GameObject item, GameObject actor)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == actor)
            {
                playerScores[i]++;
                scoresTxts[i].text = playerScores[i].ToString();
                StartCoroutine(ScoreAnimation(true, actor));
                break;
            }
        }
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
