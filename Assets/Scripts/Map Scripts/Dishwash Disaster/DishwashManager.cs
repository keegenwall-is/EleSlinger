using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DishwashManager : MinigameManager
{

    [Header("Dishwash Variables")]
    public float waitToSpawn;
    public float findPlatesTime;
    public float showSuccessTime;
    public int numOfPatterns;
    public float plateHeight;
    public float circleRadius;
    public GameObject plate;
    public int numOfRings;
    public Sprite[] icons;
    public Image mainImage;
    public List<Text> scores = new List<Text>();

    private bool spawnPlates = false;
    private bool findPlates = false;
    private bool showSuccess = false;
    private float waitToSpawnCurrent;
    private float findPlatesCurrent;
    private float showSuccessCurrent;
    private List<GameObject> plates = new List<GameObject>();
    private int patternSelection;
    private Vector3 plateSpawnPos;
    private int[] iconUsages = { 0, 0, 0, 0, 0, 0, 0, 0 };
    private int successfulIcon;
    private int[] playerScores = { 0, 0, 0, 0 };
    private float winningScore = 0;
    private GameObject winningPlayer;

    // Start is called before the first frame update
    void Start()
    {
        waitToSpawnCurrent = 3f;
        plateSpawnPos = new Vector3(0, plateHeight, 0);
        successfulIcon = Random.Range(0, icons.Length - 1);
        mainImage.sprite = icons[successfulIcon];
    }

    protected override void OnTick()
    {
        if (spawnPlates)
        {
            waitToSpawnCurrent -= Time.deltaTime;

            if (waitToSpawnCurrent <= 0)
            {
                spawnPlates = false;
                findPlates = true;
                findPlatesCurrent = findPlatesTime;
                waitToSpawnCurrent = waitToSpawn;
                patternSelection = Random.Range(1, numOfPatterns + 1);

                switch (patternSelection)
                {
                    case 1:
                        for (int ring = 1; ring < numOfRings + 1; ring++)
                        {
                            if (ring != 2)
                            {
                                for (int i = 0; i < 8 * ring; i++)
                                {
                                    float angle = i * Mathf.PI * 2f / 8f / ring;

                                    plateSpawnPos.z = Mathf.Sin(angle) * circleRadius * ring;
                                    plateSpawnPos.x = Mathf.Cos(angle) * circleRadius * ring;
                                    GameObject thisPlate = Instantiate(plate, plateSpawnPos, Quaternion.identity);
                                    plates.Add(thisPlate);
                                    SetPlateIcon(thisPlate);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 4 * ring; i++)
                                {
                                    float angle = i * Mathf.PI * 2f / 8f;

                                    plateSpawnPos.z = Mathf.Sin(angle) * circleRadius * ring;
                                    plateSpawnPos.x = Mathf.Cos(angle) * circleRadius * ring;
                                    GameObject thisPlate = Instantiate(plate, plateSpawnPos, Quaternion.identity);
                                    plates.Add(thisPlate);
                                    SetPlateIcon(thisPlate);
                                }
                            }
                        }

                        break;
                    case 2:
                        plateSpawnPos.z = -circleRadius * 2.5f;
                        for (int i = 0; i < 6; i++)
                        {
                            plateSpawnPos.x = -circleRadius * 3.5f;
                            for (int j = 0; j < 8; j++)
                            {
                                if ((i == 2 || i == 3) && (j == 3 || j == 4))
                                {
                                    plateSpawnPos.x += circleRadius;
                                    continue;
                                }

                                if ((i == 1 && j == 1) || (i == 4 && j == 1) || (i == 1 && j == 6) || (i == 4 && j == 6))
                                {
                                    plateSpawnPos.x += circleRadius;
                                    continue;
                                }

                                GameObject thisPlate = Instantiate(plate, plateSpawnPos, Quaternion.identity);
                                plates.Add(thisPlate);
                                SetPlateIcon(thisPlate);
                                plateSpawnPos.x += circleRadius;
                            }
                            plateSpawnPos.z += circleRadius;
                        }

                        break;

                    case 3:
                        plateSpawnPos.z = circleRadius * 4;
                        
                        for (int i = 0; i < 9; i++)
                        {
                            int numInRow = 0;
                            if (i >= 2 && i <= 4)
                            {
                                numInRow = i + 2;
                            }
                            else if (i == 8)
                            {
                                numInRow = 2;
                            }
                            else
                            {
                                numInRow = i;
                            }

                            plateSpawnPos.x = -circleRadius * 0.5f * numInRow;

                            for (int j = 0; j < numInRow + 1; j++)
                            {
                                if ((i == 2 && j == 2) || (i == 4 && j == 3) ||(i == 6 && j == 1) || (i == 6 && j == 5))
                                {
                                    plateSpawnPos.x += circleRadius;
                                    continue;
                                }

                                GameObject thisPlate = Instantiate(plate, plateSpawnPos, Quaternion.identity);
                                plates.Add(thisPlate);
                                SetPlateIcon(thisPlate);
                                plateSpawnPos.x += circleRadius;
                            }
                            plateSpawnPos.z -= circleRadius;
                        }

                        break;
                }
            }
        }
        else if (findPlates)
        {
            findPlatesCurrent -= Time.deltaTime;

            if (findPlatesCurrent <= 0)
            {
                findPlates = false;
                showSuccess = true;
                showSuccessCurrent = showSuccessTime;

                for (int i = plates.Count - 1; i >= 0; i--)
                {
                    if (plates[i].GetComponentInChildren<Image>().sprite != icons[successfulIcon])
                    {
                        FloatingPlatformBehaviour plateScript = plates[i].GetComponentInChildren<FloatingPlatformBehaviour>();
                        plateScript.fall = true;
                        plates.RemoveAt(i);
                    }
                }
            }
        }
        else if (showSuccess)
        {
            showSuccessCurrent -= Time.deltaTime;

            if (showSuccessCurrent <= 0)
            {
                showSuccess = false;
                spawnPlates = true;
                waitToSpawnCurrent = waitToSpawn;
                successfulIcon = Random.Range(0, icons.Length - 1);
                mainImage.sprite = icons[successfulIcon];

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
                                print(playerScores[k]);
                                scores[k].text = playerScores[k].ToString();
                                StartCoroutine(ScoreAnimation(true, players[k]));
                            }
                        }
                    }

                    plateScript.fall = true;
                    plates.RemoveAt(i);
                }

                if (overTime)
                {
                    StartCoroutine(CheckEnd());
                }

                for (int i = 0; i < iconUsages.Length; i++)
                {
                    iconUsages[i] = 0;
                }
            }
        }
    }

    private void SetPlateIcon(GameObject plate)
    {
        FloatingPlatformBehaviour plateScript = plate.GetComponentInChildren<FloatingPlatformBehaviour>();

        int usedCounter = 0;
        for (int i = 0; i < iconUsages.Length; i++)
        {
            if (iconUsages[i] >= 4)
            {
                usedCounter++;
            }
        }

        if (usedCounter == iconUsages.Length)
        {
            return;
        }

        int iconNum = Random.Range(0, icons.Length);
        while (iconUsages[iconNum] >= 4)
        {
            iconNum = Random.Range(0, icons.Length);
        }
        plateScript.SetIcon(icons[iconNum]);
        iconUsages[iconNum]++;
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
