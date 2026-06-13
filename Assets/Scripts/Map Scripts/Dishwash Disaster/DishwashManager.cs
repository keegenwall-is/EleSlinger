using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DishwashManager : MinigameManager
{

    [Header("Dishwash Variables")]
    public float waitToSpawn;
    public float showSuccessTime;
    public float plateHeight;
    public GameObject[] smallPlatforms;
    public GameObject[] bigPlatforms;
    public List<Text> scores = new List<Text>();
    public GameObject pipe;
    public float gapDist;
    public int itemCount;
    public GameObject water;
    public float waterRiseSpeed;

    public GameObject[] plateGroups;
    public float plateSpeed;
    public GameObject soaker;

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
    private bool lastWasBig = false;

    private float spawnCD;
    private float spawnCurrent;
    private List<GameObject> activePlateGroups = new List<GameObject>();
    private float lastGroupSize;

    // Start is called before the first frame update
    void Start()
    {
        waitToSpawnCurrent = 3f;
        spawnCurrent = 0f;
        //spawnCD = 10f;
        plateSpawnPos = new Vector3(0, plateHeight, 0);
        radius = gapDist / (2 * Mathf.Sin(Mathf.PI / itemCount));
        itemsPerPath = 2;
        totalItems = (int)itemCount + (itemsPerPath * 4);
        hasGracePeriod = false;
    }

    protected override void OnTick()
    {
        if (spawnPlates)
        {
            //waitToSpawnCurrent -= Time.deltaTime;

            spawnCurrent -= Time.deltaTime;

            if (spawnCurrent <= 0)
            {
                int randGroup = Random.Range(0, plateGroups.Length);
                Vector3 spawnPos = new Vector3(60f, 0f, 0f);
                GameObject thisPlateGroup = Instantiate(plateGroups[randGroup], spawnPos, Quaternion.identity);
                lastGroupSize = Variables.Object(thisPlateGroup).Get<float>("groupSize");
                spawnCurrent = lastGroupSize / plateSpeed;
                activePlateGroups.Add(thisPlateGroup);
            }

            foreach (GameObject plateGroup in activePlateGroups)
            {
                plateGroup.transform.position -= Vector3.right * plateSpeed * Time.deltaTime;
            }

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

                GenerateMap();

                ClearOldSuccess();
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
                    Transform correctMark = plates[i].transform.Find("CorrectMark");
                    if (correctMark != null)
                    {
                        if (!correctMark.gameObject.activeSelf)
                        {
                            Destroy(plates[i]);
                            plates.RemoveAt(i);
                        }
                    }
                    else
                    {
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
                    Transform correctMark = plates[i].transform.Find("CorrectMark");
                    if (correctMark != null)
                    {
                        correctMark.gameObject.SetActive(false);
                    }
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

    private void GenerateMap()
    {
        int numOfBigs = Random.Range(1, 3);
        int bigsCount = 0;
        randomCount = 0;
        randomCorrect = Random.Range(1, (totalItems / 4) - (2 * numOfBigs) - itemsPerPath);
        correctCount = 0;
        isCorrect = false;
        for (int i = 0; i < itemCount; i++)
        {
            if (lastWasBig)
            {
                lastWasBig = false;

                if ((i + 1) % (itemCount / 4) == 0 && i != 0)
                {
                    randomCount = 0;
                    bigsCount = 0;
                    numOfBigs = Random.Range(1, 3);
                    randomCorrect = Random.Range(1, (totalItems / 4) - (2 * numOfBigs) - itemsPerPath);
                }

                continue;
            }

            float angle = i * Mathf.PI * 2f / itemCount;
            bool isBig = false;
            if (i < itemCount)
            {
                float nextAngle = (i + 1) * Mathf.PI * 2 / itemCount;
                //Only allow isBig to be true if its not the last plate in the quadrant
                if ((i % (itemCount / 4) != (itemCount / 4) - 1) && bigsCount < numOfBigs && !(lastSuccessAngles.Contains(angle) || lastSuccessAngles.Contains(nextAngle)) && (randomCorrect - randomCount > 2 || randomCount > randomCorrect))
                {
                    isBig = true;
                    bigsCount++;
                    lastWasBig = true;
                }
            }

            //can use setup to re setup the paths after the center plate was correct
            //maybe change when paths are generated, do they always need to be on successful paths?
            if (setup && i % (itemCount / 4) == 0)
            {
                DrawLine(angle);
            }
            else if (lastSuccessAngles.Contains(angle))
            {
                lastSuccessAngles.Remove(angle);
                DrawLine(angle);
            }

            if (isBig)
            {
                //Find the mid point of the two angles that the big spawn will go between
                angle = (i + 0.5f) * Mathf.PI * 2f / itemCount;
                randomCorrect -= 1;
            }
            else
            {
                randomCount++;
            }

            plateSpawnPos.z = Mathf.Sin(angle) * radius;
            plateSpawnPos.x = Mathf.Cos(angle) * radius;

            isCorrect = false;
            if (randomCount == randomCorrect && !isBig)
            {
                isCorrect = true;
                correctCount++;
                lastSuccessAngles.Add(angle);
            }

            if (isBig)
            {
                SpawnBig(plateSpawnPos, false);
            }
            else
            {
                SpawnPlate(plateSpawnPos, isCorrect);
            }

            //Check if we are on the last plate or big of the quadrant
            bool endOfQuadrant = ((i + 1) % (itemCount / 4) == 0 && i != 0);
            if (isBig && ((i + 2) % (itemCount / 4) == 0))
            {
                endOfQuadrant = true;
            }

            if (endOfQuadrant)
            {
                randomCount = 0;
                bigsCount = 0;
                numOfBigs = Random.Range(1, 3);
                randomCorrect = Random.Range(1, (totalItems / 4) - (2 * numOfBigs) - itemsPerPath);
            }
        }
    }

    private void ClearOldSuccess()
    {
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

    private void SpawnPlate(Vector3 pos, bool isCorrect)
    {
        int randSmall = Random.Range(0, smallPlatforms.Length);
        GameObject thisPlate = Instantiate(smallPlatforms[randSmall], pos, Quaternion.identity);
        plates.Add(thisPlate);
        if (!isCorrect)
        {
            thisPlate.transform.Find("CorrectMark").gameObject.SetActive(false);
            thisPlate.transform.Find("Glass").gameObject.SetActive(false);
        }
    }

    private void SpawnBig(Vector3 pos, bool isLine)
    {
        Vector3 centerToPlateDirection = pos - new Vector3(0f, plateHeight, 0f);
        Quaternion spawnRot = Quaternion.LookRotation(centerToPlateDirection);
        if (isLine)
        {
            spawnRot = spawnRot * Quaternion.Euler(0f, -90f, 0f);
        }
        pos.y -= 3f;
        int randBig = Random.Range(0, bigPlatforms.Length);
        GameObject thisBig = Instantiate(bigPlatforms[randBig], pos, spawnRot);
        plates.Add(thisBig);
    }

    private void DrawLine(float angle)
    {
        Vector3 linePos = plateSpawnPos;
        bool isBig = false;
        if (Random.Range(0, 2) == 1)
        {
            isBig = true;
        }

        if (isBig)
        {
            linePos.x = Mathf.Cos(angle) * (radius - 1.5f * gapDist);
            linePos.z = Mathf.Sin(angle) * (radius - 1.5f * gapDist);
            //randomCorrect -= 1;
            SpawnBig(linePos, true);
        }
        else
        {
            for (float d = itemsPerPath; d > 0; d -= 1f)
            {
                linePos.x = Mathf.Cos(angle) * (radius - d * gapDist);
                linePos.z = Mathf.Sin(angle) * (radius - d * gapDist);
                isCorrect = false;
                //commented out to stop correct plate spawns on line
                //(i love keegen, Jess 2026)
                /*randomCount++;
                if (randomCount == randomCorrect)
                {
                    isCorrect = true;
                    correctCount++;
                    lastSuccessAngles.Add(angle);
                }*/
                SpawnPlate(linePos, isCorrect);
            }
        }
    }

    protected override void OnAllReady()
    {
        spawnPlates = true;

        foreach (GameObject player in players)
        {
            CharacterBase baseScript = player.GetComponent<CharacterBase>();
            PlayerUseItem useItemScript = player.GetComponent<PlayerUseItem>();
            baseScript.SetHasActiveItem(true);
            useItemScript.SetItem(soaker);
        }
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
