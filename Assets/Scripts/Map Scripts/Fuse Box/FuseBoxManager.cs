using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FuseBoxManager : MinigameManager
{

    public List<Text> scores = new List<Text>();

    private int[] playerScores = { -1, -1, -1, -1 };

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerNo; i++)
        {
            playerScores[i] = 0;
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
