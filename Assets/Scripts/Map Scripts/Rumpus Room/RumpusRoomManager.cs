using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RumpusRoomManager : MinigameManager
{
    public int maxLives;
    public List<Text> livesTxts = new List<Text>();

    private int[] lives = { 0, 0, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            lives[i] = maxLives;
            livesTxts[i].text = maxLives.ToString();
        }
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                lives[i]--;

                if (lives[i] <= 0)
                {
                    CharacterBase thisPlayerBase = player.gameObject.GetComponent<CharacterBase>();
                    thisPlayerBase.SetState(CharacterBase.playerState.Out);
                }
                else
                {
                    GameObject spawn = SetPlayerSpawn(player);
                    KillPlayer(player, spawn);
                }

                UpdateScore(player);
                CheckGameEnd();
            }
        }
    }

    private void UpdateScore(GameObject player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                livesTxts[i].text = lives[i].ToString();
            }
        }
    }

    private void CheckGameEnd()
    {
        int stillIn = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].tag != "Out")
            {
                stillIn++;
            }
        }

        if (stillIn == 1)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].tag != "Out")
                {
                    gameController.IncreaseRoundWins(players[i]);
                }
            }
        }
    }
}
