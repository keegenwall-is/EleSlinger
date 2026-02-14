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
    public GameObject popsicleCube;
    public GameObject popsicleFrost;
    public float iceSpawnCD;
    public float popsicleSpawnCD;
    public GameObject[] goalBlockers;
    public int iceMashes;

    private int[] playerScores = { -1, -1, -1, -1 };
    private float iceSpawnCurrent;
    private float popsicleSpawnCurrent;
    private int randIceSpawn;
    private int randPopSpawn;
    private float winningScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Player scores of players who aren't playing are still -1 and so won't affect game logic
        for (int i = 0; i < playerNo; i++)
        {
            playerScores[i] = 0;
        }

        if (playerNo == 3)
        {
            Destroy(goalBlockers[0]);
        } else if (playerNo == 4)
        {
            Destroy(goalBlockers[0]);
            Destroy(goalBlockers[1]);
        }
        //Get rid of the goals that arent being used;
    }

    protected override void OnTick()
    {
        iceSpawnCurrent += 1 * Time.deltaTime;
        popsicleSpawnCurrent += 1 * Time.deltaTime;

        if (iceSpawnCurrent >= iceSpawnCD)
        {
            iceSpawnCurrent = 0;
            randIceSpawn = Random.Range(0, iceSpawners.Length);
            Instantiate(iceCube, iceSpawners[randIceSpawn].transform);
        }

        if (popsicleSpawnCurrent >= popsicleSpawnCD)
        {
            popsicleSpawnCurrent = 0;
            randPopSpawn = Random.Range(0, 4);
            Instantiate(popsicle, popsicleSpawners[randPopSpawn].transform);
        }

        if (overTime)
        {
            OnMinigameEnd();
        }
    }

    protected override void OnInteractiveObjectEvent(GameObject obj, GameObject player, GameObject other)
    {
        //increase score for player who shot the goal and decrease for the goal scored against

        for (int i = 0; i < players.Count; i++)
        {
            /*if (goals[i] == other)
            {
                if (playerScores[i] > 0)
                {
                    playerScores[i]--;
                    scores[i].text = playerScores[i].ToString();
                    StartCoroutine(ScoreAnimation(false, players[i]));
                }
            } else if (players[i] == player)
            {
                playerScores[i]++;
                scores[i].text = playerScores[i].ToString();
                StartCoroutine(ScoreAnimation(true, player));
            }*/

        if (players[i] == player)
            {
                playerScores[i]++;
                scores[i].text = playerScores[i].ToString();
                StartCoroutine(ScoreAnimation(true, player));
            }
        }

        randIceSpawn = Random.Range(0, iceSpawners.Length);
        Instantiate(iceCube, iceSpawners[randIceSpawn].transform);
        iceSpawnCurrent = 0;
    }

    public override void HandleItemPickup(GameObject item, GameObject actor)
    {
        PlayerAttack attackScript = actor.GetComponent<PlayerAttack>();
        attackScript.SetSpecialAttack(true);

        Vector3 spawnPos = actor.transform.position;
        spawnPos.y += 4.0f;
        GameObject thisPopFrost = Instantiate(popsicleFrost, spawnPos, actor.transform.rotation, actor.transform);
        thisPopFrost.transform.localScale /= 50;
    }

    public override void HandleSpecialAttack(GameObject hitPlayer, GameObject thrower)
    {
        Vector3 spawnPos = hitPlayer.transform.position;
        spawnPos.y += 2.5f;
        GameObject thisIce = Instantiate(popsicleCube, spawnPos, hitPlayer.transform.rotation);
        hitPlayer.transform.SetParent(thisIce.transform);
        IceCubeBehaviour iceScript = thisIce.GetComponent<IceCubeBehaviour>();
        iceScript.SetWillShrink(false);
        iceScript.SetAttachedPlayer(hitPlayer);

        PlayerStunned stunnedScript = hitPlayer.gameObject.GetComponent<PlayerStunned>();
        stunnedScript.SetMashes(iceMashes);
        stunnedScript.Stunned();

        CapsuleCollider hitPlayerCC = hitPlayer.GetComponent<CapsuleCollider>();
        hitPlayerCC.enabled = false;

        hitPlayer.transform.position = thisIce.transform.position;

        PlayerAttack attackScript = thrower.GetComponent<PlayerAttack>();
        attackScript.SetSpecialAttack(false);

        List<GameObject> frosts = new List<GameObject>();

        foreach (Transform child in thrower.transform)
        {
            if (child.name.Contains("Pop"))
            {
                frosts.Add(child.gameObject);
            }
        }

        foreach (GameObject frost in frosts)
        {
            Destroy(frost);
        }
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
                    gameController.IncreaseRoundWins(players[i]);
                    return;
                }
                else if (playerScores[i] == winningScore)
                {
                    maxScoreCounter++;
                }
            }

            if (maxScoreCounter == 1)
            {
                for (int i = 0; i < playerScores.Length; i++)
                {
                    if (playerScores[i] == winningScore)
                    {
                        overTime = false;
                        gameController.IncreaseRoundWins(players[i]);
                    }
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
