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
    public float addCubeCD = 30f;
    public List<GameObject> iceCubes = new List<GameObject>();
    public Image[] readyBackgrounds;
    public Sprite blueReadyBackground;
    public Sprite redReadyBackground;
    public RectTransform[] profilePoss;
    public Canvas canvas;

    private int[] playerScores = { 0, 0 };
    private float popsicleSpawnCurrent;
    private float snowManSpawnCurrent;
    private int randIceSpawn;
    private int randPopSpawn;
    private float winningScore = 0;
    private CameraMovement camMoveScript;
    private float randPopSpawnCD;
    private float randSnowManSpawnCD;
    private float addCubeCurrent;

    // Start is called before the first frame update
    void Start()
    {
        iceCubes.Add(startIce);

        if (playerNo <= 2)
        {
            Vector3 spawnPos1 = playerSpawners[0].transform.position;
            spawnPos1.z = 0;
            playerSpawners[0].transform.position = spawnPos1;
            Vector3 spawnPos2 = playerSpawners[1].transform.position;
            spawnPos2.z = 0;
            playerSpawners[1].transform.position = spawnPos2;
        }

        if (playerNo > 2)
        {
            List<int> shuffledPlayers = new List<int>();
            if (playerNo == 3)
            {
                shuffledPlayers = new List<int> { 0, 1, 2 };
            }
            else
            {
                shuffledPlayers = new List<int> { 0, 1, 2, 3 };
            }

            for (int i = shuffledPlayers.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (shuffledPlayers[i], shuffledPlayers[j]) = (shuffledPlayers[j], shuffledPlayers[i]);
            }

            for (int i = 0; i < playerNo; i++)
            {
                if (i % 2 == 0)
                {
                    team1.Add(shuffledPlayers[i]);
                }
                else
                {
                    team2.Add(shuffledPlayers[i]);
                }
            }
        }
        else if (playerNo == 2)
        {
            team1.Add(0);
            team2.Add(1);
        }
        else if (playerNo == 1)
        {
            team1.Add(0);
        }

        for (int i = 0; i < readyBackgrounds.Length; i++)
        {
            if (team1.Contains(i))
            {
                readyBackgrounds[i].sprite = blueReadyBackground;
                Vector3 spawnPosSpawnPos = playerSpawners[i].transform.position;
                Vector2 profilePos = profilePoss[i].anchoredPosition;
                spawnPosSpawnPos.x = -45f;

                if (playerNo > 2)
                {
                    spawnPosSpawnPos.z = (team1[team1.Count - 1] == i) ? -15f : 15f;
                }
                profilePos.x = (team1[team1.Count - 1] == i) ? -290f : -350f;

                playerSpawners[i].transform.position = spawnPosSpawnPos;
                playerSpawners[i].transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                profilePoss[i].anchoredPosition = profilePos;
            }
            else if (team2.Contains(i))
            {
                readyBackgrounds[i].sprite = redReadyBackground;
                Vector3 spawnPosSpawnPos = playerSpawners[i].transform.position;
                Vector2 profilePos = profilePoss[i].anchoredPosition;
                spawnPosSpawnPos.x = 45f;

                if (playerNo > 3)
                {
                    spawnPosSpawnPos.z = (team2[team2.Count - 1] == i) ? -15f : 15f;
                }
                else if (playerNo == 3)
                {
                    spawnPosSpawnPos.z = 0f;
                }

                profilePos.x = (team2[team2.Count - 1] == i) ? 290f : 350f;

                playerSpawners[i].transform.position = spawnPosSpawnPos;
                playerSpawners[i].transform.rotation = Quaternion.Euler(0f, 270f, 0f);
                profilePoss[i].anchoredPosition = profilePos;
            }
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
        popsicleSpawnCurrent += Time.deltaTime;
        snowManSpawnCurrent += Time.deltaTime;
        addCubeCurrent += Time.deltaTime;

        if (popsicleSpawnCurrent >= randPopSpawnCD)
        {
            popsicleSpawnCurrent = 0f;
            randPopSpawn = Random.Range(0, 4);
            Instantiate(popsicle, popsicleSpawners[randPopSpawn].transform);
            randPopSpawnCD = Random.Range(popsicleSpawnCD - randSpawnAmount, popsicleSpawnCD + randSpawnAmount);
        }

        if (snowManSpawnCurrent >= randSnowManSpawnCD)
        {
            snowManSpawnCurrent = 0f;
            Vector3 spawnPos = new Vector3(Random.Range(-30f, 30f), 50f, Random.Range(-30f, 30f));
            GameObject thisSnowMan = Instantiate(snowMan, spawnPos, Quaternion.identity);
            randSnowManSpawnCD = Random.Range(snowManSpawnCD - randSpawnAmount, snowManSpawnCD + randSpawnAmount);
        }

        if (addCubeCurrent >= addCubeCD)
        {
            addCubeCurrent = 0f;
            GameObject thisIce = Instantiate(iceCube, iceSpawners[randIceSpawn].transform);
            camMoveScript.FindObject(thisIce);
            iceCubes.Add(thisIce);
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
        iceCubes.Remove(obj);
        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i] == other)
            {
                addCubeCurrent = 0f;
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
        if (attackScript.GetSpecialAttack())
        {
            return;
        }
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
        IceNovaBehaviour novaScript = nova.GetComponent<IceNovaBehaviour>();
        novaScript.SetThrower(thrower);
        int throwerIndex = players.IndexOf(thrower);
        if (throwerIndex != -1)
        {
            if (team1.Contains(throwerIndex) && team1.Count > 1)
            {
                for (int i = 0; i < team1.Count; i++)
                {
                    if (team1[i] != throwerIndex)
                    {
                        novaScript.SetTeamMate(players[team1[i]]);
                    }
                }
            }
            else if (team2.Contains(throwerIndex) && team2.Count > 1)
            {
                for (int i = 0; i < team2.Count; i++)
                {
                    if (team2[i] != throwerIndex)
                    {
                        novaScript.SetTeamMate(players[team2[i]]);
                    }
                }
            }
        }

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
        iceCubes.Add(thisIce);
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
                    if (i == 0)
                    {
                        TeamWin(team1);
                    }
                    else
                    {
                        TeamWin(team2);
                    }
                    return;
                }
                else if (playerScores[i] == winningScore)
                {
                    maxScoreCounter++;
                }
            }

            if (maxScoreCounter == 1)
            {
                if (playerScores[0] == winningScore)
                {
                    TeamWin(team1);
                    overTime = false;
                }
                else if (playerScores[1] == winningScore)
                {
                    TeamWin(team2);
                    overTime = false;
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
                    TeamWin(team1);
                }
                else if (playerScores[1] == winningScore)
                {
                    TeamWin(team2);
                }
                gameUI.SetActive(false);
            }
        }
    }

    private void TeamWin(List<int> team)
    {
        List<GameObject> victoriousPlayers = new List<GameObject>();
        for (int i = 0; i < team.Count; i++)
        {
            victoriousPlayers.Add(players[team[i]]);
        }
        gameController.IncreaseRoundWins(victoriousPlayers.ToArray());
    }
}
