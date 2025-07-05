using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    private CapsuleCollider cc;
    private MinigameManager managerScript;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CapsuleCollider>();
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<MinigameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Finding the largest, minimum player distance from any spawner
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            List<GameObject> players = new List<GameObject>();
            players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
            players.AddRange(GameObject.FindGameObjectsWithTag("Immune"));
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

            CharacterBase thisPlayerBase = other.gameObject.GetComponent<CharacterBase>();
            if (thisPlayerBase != null)
            {
                if (thisPlayerBase.GetState() == CharacterBase.playerState.Attacking)
                {
                    PlayerAttack thisPlayerAttack = other.gameObject.GetComponent<PlayerAttack>();
                    thisPlayerAttack.CancelAttack();
                }
                thisPlayerBase.SetState(CharacterBase.playerState.Idle);
                thisPlayerBase.SetSpawnPos(spawnPoint.transform.position);
                thisPlayerBase.SetState(CharacterBase.playerState.Dead);
            }
            
            if (transform.parent != null)
            {
                if (transform.parent.gameObject.tag == "Immune")
                {
                    TakeHit thisTakeHit = other.gameObject.GetComponent<TakeHit>();
                    thisTakeHit.SetAttacker(transform.parent.gameObject);
                }
            }

            managerScript.TriggerObstacleEvent(other.gameObject);
        }
    }
}
