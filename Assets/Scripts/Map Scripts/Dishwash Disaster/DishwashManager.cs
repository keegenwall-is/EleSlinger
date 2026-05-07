using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DishwashManager : MinigameManager
{

    [Header("Dishwash Variables")]
    public float waitToSpawn;
    public float showSuccessTime;
    public float plateHeight;
    public GameObject plate;
    public List<Text> scores = new List<Text>();
    public GameObject pipe;
    public float gapDist;
    public float itemCount;
    public GameObject water;
    public float waterRiseSpeed;

    private bool spawnPlates = false;
    private bool findPlates = false;
    private bool showSuccess = false;
    private float waitToSpawnCurrent;
    private float showSuccessCurrent;
    private List<GameObject> plates = new List<GameObject>();
    private Vector3 plateSpawnPos;
    private int[] playerScores = { 0, 0, 0, 0 };
    private float winningScore = 0;
    private GameObject winningPlayer;
    private List<float> lastSuccessAngles = new List<float>();
    private bool setup = true;
    private float radius;
    private int itemsPerPath;
    private int totalItems;
    private int randomCount;
    private int randomCorrect;
    private int correctCount;
    private bool isCorrect;
    private List<GameObject> oldSuccess = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        waitToSpawnCurrent = 3f;
        plateSpawnPos = new Vector3(0, plateHeight, 0);
        radius = gapDist / (2 * Mathf.Sin(Mathf.PI / itemCount));
        itemsPerPath = (int)(radius / gapDist);
        totalItems = (int)itemCount + (itemsPerPath * 4);
    }

    protected override void OnTick()
    {
        if (spawnPlates)
        {
            waitToSpawnCurrent -= Time.deltaTime;

            if (water.transform.position.y > 0)
            {
                water.transform.position -= Vector3.up * waterRiseSpeed * 2f * Time.deltaTime;
            }

            if (waitToSpawnCurrent <= 0)
            {
                water.transform.position = Vector3.zero;
                spawnPlates = false;
                findPlates = true;
                waitToSpawnCurrent = waitToSpawn;

                randomCount = 0;
                randomCorrect = Random.Range(1, totalItems / 4);
                correctCount = 0;
                isCorrect = false;

                for (int i = 0; i < itemCount; i++)
                {
                    float angle = i * Mathf.PI * 2f / itemCount;
                    if (setup && i % (itemCount / 4) == 0)
                    {
                        DrawLine(angle);
                    }
                    else if (lastSuccessAngles.Contains(angle))
                    {
                        lastSuccessAngles.Remove(angle);
                        DrawLine(angle);
                    }

                    plateSpawnPos.z = Mathf.Sin(angle) * radius;
                    plateSpawnPos.x = Mathf.Cos(angle) * radius;

                    isCorrect = false;
                    randomCount++;
                    if (randomCount == randomCorrect)
                    {
                        isCorrect = true;
                        correctCount++;
                        lastSuccessAngles.Add(angle);
                    }
                    SpawnPlate(plateSpawnPos, isCorrect);
                    if ((i + 1) % (itemCount / 4) == 0 && i != 0)
                    {
                        randomCount = 0;
                        randomCorrect = Random.Range(1, totalItems / 4);
                    }
                }

                for (int i = oldSuccess.Count - 1; i >= 0; i--)
                {
                    FloatingPlatformBehaviour platformScript = oldSuccess[i].GetComponentInChildren<FloatingPlatformBehaviour>();
                    if (platformScript.onPlayers != null)
                    {
                        foreach (GameObject player in platformScript.onPlayers)
                        {
                            player.tag = "Player";
                        }
                    }
                    Destroy(oldSuccess[i]);
                    oldSuccess.RemoveAt(i);
                }
            }
        }
        else if (findPlates)
        {
            if (water.transform.position.y <= 47f)
            {
                water.transform.position += Vector3.up * waterRiseSpeed * Time.deltaTime;
            }
            else
            {
                findPlates = false;
                showSuccess = true;
                showSuccessCurrent = showSuccessTime;

                for (int i = plates.Count - 1; i >= 0; i--)
                {
                    if (!plates[i].transform.Find("CorrectMark").gameObject.activeSelf)
                    {
                        FloatingPlatformBehaviour plateScript = plates[i].GetComponentInChildren<FloatingPlatformBehaviour>();
                        Destroy(plates[i]);
                        plates.RemoveAt(i);
                    }
                }
            }
        }
        else if (showSuccess)
        {
            showSuccessCurrent -= Time.deltaTime;

            if (water.transform.position.y > 0)
            {
                water.transform.position -= Vector3.up * waterRiseSpeed * 2f * Time.deltaTime;
            }

            if (showSuccessCurrent <= 0)
            {
                setup = false;
                showSuccess = false;
                spawnPlates = true;
                waitToSpawnCurrent = waitToSpawn;

                for (int i = plates.Count - 1; i >= 0; i--)
                {
                    FloatingPlatformBehaviour plateScript = plates[i].GetComponentInChildren<FloatingPlatformBehaviour>();

                    for (int j = 0; j < plateScript.onPlayers.Count; j++)
                    {
                        for (int k = 0; k < players.Count; k++)
                        {
                            if (plateScript.onPlayers[j] == players[k])
                            {
                                playerScores[k]++;
                                scores[k].text = playerScores[k].ToString();
                                StartCoroutine(ScoreAnimation(true, players[k]));
                            }
                        }
                    }
                    plates[i].transform.Find("CorrectMark").gameObject.SetActive(false);
                    oldSuccess.Add(plates[i]);
                    plates.RemoveAt(i);
                }

                if (overTime)
                {
                    StartCoroutine(CheckEnd());
                }
            }
        }
    }

    private void SpawnPlate(Vector3 pos, bool isCorrect)
    {
        GameObject thisPlate = Instantiate(plate, pos, Quaternion.identity);
        plates.Add(thisPlate);
        if (!isCorrect)
        {
            thisPlate.transform.Find("CorrectMark").gameObject.SetActive(false);
            thisPlate.transform.Find("Glass").gameObject.SetActive(false);
        }
    }

    private void DrawLine(float angle)
    {
        for (float d = radius - gapDist; d > 0.1f; d -= gapDist)
        {
            Vector3 linePos = plateSpawnPos;
            linePos.x = Mathf.Cos(angle) * d;
            linePos.z = Mathf.Sin(angle) * d;
            isCorrect = false;
            randomCount++;
            if (randomCount == randomCorrect)
            {
                isCorrect = true;
                correctCount++;
                lastSuccessAngles.Add(angle);
            }
            SpawnPlate(linePos, isCorrect);
        }
    }

    protected override void OnAllReady()
    {
        spawnPlates = true;
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        GameObject spawn = SetPlayerSpawn(player);
        KillPlayer(player, spawn);
    }

    private IEnumerator CheckEnd()
    {
        yield return new WaitForSeconds(1f);

        OnMinigameEnd();
    }

    protected override void OnMinigameEnd()
    {
        if (overTime)
        {
            //As soon as a player beats the winning score or only 1 player is left with the winning score, the game ends
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
                    winningPlayer = players[i];
                }
            }

            if (maxScoreCounter == 1)
            {
                overTime = false;
                gameController.IncreaseRoundWins(winningPlayer);
            }
        }
        else
        {
            overTime = false;

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
