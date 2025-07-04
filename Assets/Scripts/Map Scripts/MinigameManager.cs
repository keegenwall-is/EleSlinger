using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

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
    public GameObject readyMenu;
    public GameObject gameUI;

    private bool roundOver = false;
    private bool roundBegun = false;
    private bool[] playersReady = { false, false, false, false };
    private List<InputDevice> playerControllers = new List<InputDevice>();
    private GameObject[] playerInterfaces;
    private List<Text> readyTexts = new List<Text>();

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("Game Controller").GetComponent<GameController>();
        players = gameController.GetPlayers();
        playerNo = gameController.GetPlayerNo();
        playerControllers = gameController.GetControllers();
        GameObject[] readyTextObjects = GameObject.FindGameObjectsWithTag("Ready Text");
        readyTexts = new List<Text>();
        foreach (GameObject obj in readyTextObjects)
        {
            Text text = obj.GetComponent<Text>();
            if (text != null)
            {
                readyTexts.Add(text);
            }
        }
        gameLengthStart = gameLength;
        ActivateUI("Ready Check");
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

    private void ActivateUI(string UIType)
    {
        if (playerNo != 0)
        {
            playerInterfaces = GameObject.FindGameObjectsWithTag(UIType);

            DeactivateUnusedUI(playerInterfaces);

        }
    }

    private void DeactivateUnusedUI(GameObject[] UIElements)
    {
        if (UIElements != null)
        {
            for (int i = 0; i < 4/*max player number*/; i++)
            {
                if (i > playerNo - 1)
                {
                    UIElements[i].SetActive(false);
                }
            }
        }
    }

    private void CheckReady()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (playerControllers[i] is Keyboard keyboard)
            {
                if (keyboard.enterKey.wasPressedThisFrame)
                {
                    if (playersReady[i] == false)
                    {
                        playersReady[i] = true;
                        readyTexts[i].text = "Ready";
                    }
                    else
                    {
                        playersReady[i] = false;
                        readyTexts[i].text = "Not Ready";
                    }
                }
            }
            else if (playerControllers[i] is Gamepad gamepad)
            {
                if (gamepad.buttonEast.wasPressedThisFrame)
                {
                    if (playersReady[i] == false)
                    {
                        playersReady[i] = true;
                        readyTexts[i].text = "Ready";
                    }
                    else
                    {
                        playersReady[i] = false;
                        readyTexts[i].text = "Not Ready";
                    }
                }
            }
        }

        bool allReady = true;
        for (int i = 0; i < playerNo; i++)
        {
            if (!playersReady[i])
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            roundBegun = true;
            readyMenu.SetActive(false);
            gameUI.SetActive(true);
            ActivateUI("Player UI");
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
