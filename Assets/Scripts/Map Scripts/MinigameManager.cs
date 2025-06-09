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

    private GameController gameController;
    private bool roundOver = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("Game Controller").GetComponent<GameController>();
        players = gameController.GetPlayers();
        playerNo = gameController.GetPlayerNo();
    }

    // Update is called once per frame
    void Update()
    {
        if (!roundOver)
        {
            if (gameLength >= 0)
            {
                gameLength -= Time.deltaTime;
                TimeSpan timeText = TimeSpan.FromSeconds(gameLength);
                countdown.text = string.Format("{0}:{1:00}", timeText.Minutes, timeText.Seconds);
            }
            else
            {
                roundOver = true;
            }
        }
    }

    public void TriggerObstacleEvent(GameObject actor)
    {
        OnObstacleEvent(actor);
    }

    protected virtual void OnObstacleEvent(GameObject actor)
    {
        //Default is to do nothing (obstacles don't have the same effect in all minigames)
    }

    protected virtual void OnMinigameEnd()
    {
        //Show end game screen
    }
}
