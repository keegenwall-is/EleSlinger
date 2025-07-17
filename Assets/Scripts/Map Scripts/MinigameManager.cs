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
    public GameObject[] spawnPoints;

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
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (readyMenu != null)
        {
            ActivateUI("Ready Check");
        }
        else
        {
            gameController.SpawnPlayers();
            ActivateUI("Player UI");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!roundBegun)
        {
            if (readyMenu != null)
            {
                CheckReady();
            }
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
        for (int i = 0; i < playerControllers.Count; i++)
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
            gameController.SpawnPlayers();
            ActivateUI("Player UI");
        }
    }

    public void KillPlayer(GameObject player, GameObject spawnPoint)
    {
        //Cancel player attack if charging
        CharacterBase thisPlayerBase = player.gameObject.GetComponent<CharacterBase>();
        if (thisPlayerBase != null)
        {
            if (thisPlayerBase.GetState() == CharacterBase.playerState.Attacking)
            {
                PlayerAttack thisPlayerAttack = player.gameObject.GetComponent<PlayerAttack>();
                thisPlayerAttack.CancelAttack();
            }
            thisPlayerBase.SetState(CharacterBase.playerState.Idle);
            thisPlayerBase.SetSpawnPos(spawnPoint.transform.position);
            thisPlayerBase.SetState(CharacterBase.playerState.Dead);
        }
    }

    public GameObject SetPlayerSpawn(GameObject player)
    {
        //Finding the largest, minimum player distance from any spawner
        float maxSpawnDist = 0;
        GameObject spawnPoint = spawnPoints[0];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float minPlayerDist = float.MaxValue;
            for (int j = 0; j < players.Count; j++)
            {
                float thisDist = Vector3.Distance(spawnPoints[i].transform.position, players[j].transform.position);
                if (thisDist < minPlayerDist)
                {
                    minPlayerDist = thisDist;
                }
            }

            //Check if the minimum player distance to each spawner is greater than to other spawners
            if (minPlayerDist > maxSpawnDist)
            {
                maxSpawnDist = minPlayerDist;
                spawnPoint = spawnPoints[i];
            }
        }

        return spawnPoint;
    }

    public void TriggerObstacleEvent(GameObject actor)
    {
        OnObstacleEvent(actor);
    }

    public void TriggerInteractiveObjectEvent(GameObject obj)
    {
        OnInteractiveObjectEvent(obj);
    }

    protected virtual void OnTick()
    {
        //Default is do nothing
    }

    protected virtual void OnObstacleEvent(GameObject actor)
    {
        //Default is to do nothing (obstacles don't have the same effect in all minigames)
    }

    protected virtual void OnInteractiveObjectEvent(GameObject obj)
    {
        //Default is do nothing
    }

    public virtual void HandleItemPickup(GameObject item, GameObject actor)
    {

    }

    protected virtual void OnMinigameEnd()
    {
        //Determine winner
    }
}
