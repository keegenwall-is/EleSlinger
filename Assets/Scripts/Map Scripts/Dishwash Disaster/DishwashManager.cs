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
    public float plateSpawnX;
    public int bigPlateScore;
    public GameObject floodWarning;

    public GameObject[] plateGroups;
    public float plateSpeed;
    public GameObject soaker;
    public GameObject rain;
    public GameObject rainSplashes;

    private bool spawnPlates = false;
    private bool waterRise = false;
    private bool waterSink = false;
    private int[] playerScores = { 0, 0, 0, 0 };
    private float winningScore = 0;
    private GameObject winningPlayer;

    private float spawnCurrent;
    private float untilWashEventCurrent;
    private List<GameObject> activePlateGroups = new List<GameObject>();
    private float lastGroupSize;
    private float origPlateSpeed;
    private bool nextIsBig = false;
    private GameObject successPlate;
    private GameObject correctMark;
    private GameObject glass;
    private bool canStartEvent = true;
    private int lastPlateGroup = -1;

    // Start is called before the first frame update
    void Start()
    {
        spawnCurrent = 0f;
        untilWashEventCurrent = Random.Range(15f, 30f);
        hasGracePeriod = false;
        origPlateSpeed = plateSpeed;
        plateHeight = 0f;
    }

    protected override void OnTick()
    {
        if (spawnPlates)
        {
            spawnCurrent -= Time.deltaTime;

            if (spawnCurrent <= 0)
            {
                GameObject thisPlateGroup = null;
                Vector3 spawnPos = new Vector3(plateSpawnX, 0f, 0f);
                if (nextIsBig)
                {
                    nextIsBig = false;
                    float distance = Random.Range(130f, 230f);
                    float time = distance / plateSpeed;
                    StartCoroutine(WaterWarning(time));
                    thisPlateGroup = Instantiate(plateGroups[1], spawnPos, Quaternion.identity);
                    successPlate = thisPlateGroup.transform.Find("Big Plate Platform").gameObject;
                }
                else
                {
                    int randGroup = Random.Range(0, plateGroups.Length);
                    while (lastPlateGroup == randGroup)
                    {
                        randGroup = Random.Range(0, plateGroups.Length);
                    }
                    lastPlateGroup = randGroup;
                    thisPlateGroup = Instantiate(plateGroups[randGroup], spawnPos, Quaternion.identity);
                }
                
                lastGroupSize = Variables.Object(thisPlateGroup).Get<float>("groupSize");
                spawnCurrent = lastGroupSize / plateSpeed;
                activePlateGroups.Add(thisPlateGroup);
            }

            for (int i = activePlateGroups.Count - 1; i >= 0; i--)
            {
                activePlateGroups[i].transform.position -= Vector3.right * plateSpeed * Time.deltaTime;

                if (activePlateGroups[i].transform.position.x < -plateSpawnX - 40)
                {
                    GameObject plateToDestroy = activePlateGroups[i];
                    activePlateGroups.RemoveAt(i);
                    Destroy(plateToDestroy);
                }
            }

            untilWashEventCurrent -= Time.deltaTime;

            if (untilWashEventCurrent <= 0 && canStartEvent)
            {
                nextIsBig = true;
                canStartEvent = false;
            }
        }
        else if (waterRise)
        {
            if (water.transform.position.y <= 15f)
            {
                water.transform.position += Vector3.up * waterRiseSpeed * Time.deltaTime;
            }
            else
            {
                waterRise = false;
                waterSink = true;
            }
        }
        else if (waterSink)
        {
            if (water.transform.position.y >= 0f)
            {
                water.transform.position -= Vector3.up * waterRiseSpeed * Time.deltaTime;
            }
            else
            {
                waterSink = false;
                spawnPlates = true;
                //I tried to implement this but it causes a delay in the spawn of the next plate, so will have to be done
                //elsewhere, maybe trigger a boolean here that causes the speed to increase after the next item has been spawned so there
                //is no gap
                //origPlateSpeed += 2.5f;
                plateSpeed = origPlateSpeed;
                rain.SetActive(false);
                rainSplashes.SetActive(false);
                untilWashEventCurrent = Random.Range(15f, 30f);
                correctMark.SetActive(false);
                glass.GetComponent<GlassBehaviour>().glassSpeed *= -1f;
                canStartEvent = true;
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

        float i = plateSpawnX;
        while (i > -plateSpawnX)
        {
            int randGroup = Random.Range(0, plateGroups.Length);
            while (lastPlateGroup == randGroup)
            {
                randGroup = Random.Range(0, plateGroups.Length);
            }
            lastPlateGroup = randGroup;
            float currentSize = Variables.Object(plateGroups[randGroup]).Get<float>("groupSize");
            i -= currentSize;
            Vector3 spawnPos = new Vector3(i, 0f, 0f);
            GameObject thisPlateGroup = Instantiate(plateGroups[randGroup], spawnPos, Quaternion.identity);
            activePlateGroups.Add(thisPlateGroup);
        }

        int randGroupInit = Random.Range(0, plateGroups.Length);
        while (lastPlateGroup == randGroupInit)
        {
            randGroupInit = Random.Range(0, plateGroups.Length);
        }
        lastPlateGroup = randGroupInit;
        float currentSizeInit = Variables.Object(plateGroups[randGroupInit]).Get<float>("groupSize");
        Vector3 spawnInit = new Vector3(plateSpawnX, 0f, 0f);
        GameObject thisPlateGroupInit = Instantiate(plateGroups[randGroupInit], spawnInit, Quaternion.identity);
        activePlateGroups.Add(thisPlateGroupInit);
        spawnCurrent = currentSizeInit / plateSpeed;
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        GameObject spawn = SetPlayerSpawn(player);
        KillPlayer(player, spawn);
    }

    public void IncreaseScoreFor(GameObject player, bool isBig)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                if (isBig)
                {
                    playerScores[i] += bigPlateScore;
                    StartCoroutine(ScoreAnimation(true, players[i], bigPlateScore));
                }
                else
                {
                    playerScores[i]++;
                    StartCoroutine(ScoreAnimation(true, players[i]));
                }
                scores[i].text = playerScores[i].ToString();
            }
        }
    }

    private IEnumerator WaterWarning(float timeUntilPause)
    {
        yield return new WaitForSeconds(timeUntilPause);

        floodWarning.SetActive(true);
        spawnPlates = false;
        plateSpeed = 0f;
        rain.SetActive(true);
        rainSplashes.SetActive(true);

        correctMark = successPlate.transform.Find("CorrectMark").gameObject;
        correctMark.SetActive(true);

        yield return new WaitForSeconds(9.0f);

        glass = successPlate.transform.Find("Glass").gameObject;
        glass.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        waterRise = true;
        floodWarning.SetActive(false);
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
