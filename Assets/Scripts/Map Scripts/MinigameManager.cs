using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MinigameManager : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    public int playerNo;
    public List<GameObject> effectedObjects = new List<GameObject>();
    public Text countdown;
    public float gameLength;
    public float gameLengthStart;
    public GameController gameController;
    public bool overTime = false;

    private bool roundOver = false;
    private bool roundBegun = false;
    private int noPlayersReady = 0;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("Game Controller").GetComponent<GameController>();
        players = gameController.GetPlayers();
        playerNo = gameController.GetPlayerNo();
        gameLengthStart = gameLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (!roundBegun)
        {
            CheckReady();
        }
        else
        {

            if (!roundOver)
            {
                if (gameLength >= 0)
                {
                    //Handles round timer
                    gameLength -= Time.deltaTime;
                    TimeSpan timeText = TimeSpan.FromSeconds(gameLength);
                    countdown.text = string.Format("{0}:{1:00}", timeText.Minutes, timeText.Seconds);
                    OnTick();
                }
                else
                {
                    roundOver = true;
                    OnMinigameEnd();
                }
            }

            if (overTime && gameLength <= 0)
            {
                OnTick();
            }
        }
    }

    private void CheckReady()
    {
        for (int i = 0; i < players.Count; i++)
        {
            /*if ()
            {

            }*/
        }

        if (noPlayersReady == players.Count)
        {
            roundBegun = true;
        }
    }

    public void TriggerObstacleEvent(GameObject actor)
    {
        OnObstacleEvent(actor);
    }

    protected virtual void OnTick()
    {
        //Default is do nothing
    }

    protected virtual void OnObstacleEvent(GameObject actor)
    {
        //Default is to do nothing (obstacles don't have the same effect in all minigames)
    }

    public virtual void HandleItemPickup(GameObject item, GameObject actor)
    {

    }

    protected virtual void OnMinigameEnd()
    {
        //Determine winner
    }
}
