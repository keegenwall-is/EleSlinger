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
    public float iceSpawnCD;
    public float popsicleSpawnCD;
    public GameObject[] goalBlockers;
    public int iceMashes;

    private int[] playerScores = { -1, -1, -1, -1 };
    private float iceSpawnCurrent;
    private float popsicleSpawnCurrent;
    private int randIceSpawn;
    private int randPopSpawn;

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
    }

    protected override void OnInteractiveObjectEvent(GameObject obj, GameObject player, GameObject other)
    {
        //increase score for player who shot the goal and decrease for the goal scored against

        for (int i = 0; i < players.Count; i++)
        {
            if (goals[i] == other)
            {
                if (playerScores[i] > 0)
                {
                    playerScores[i]--;
                    scores[i].text = playerScores[i].ToString();
                }
            } else if (players[i] == player)
            {
                playerScores[i]++;
                scores[i].text = playerScores[i].ToString();
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
    }
}
