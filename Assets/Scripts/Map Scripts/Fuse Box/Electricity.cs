using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
{

    private CapsuleCollider cc;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Finding the largest, minimum player distance from any spawner
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            float maxSpawnDist = 0;
            GameObject spawnPoint = spawnPoints[0];

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                float minPlayerDist = float.MaxValue;
                for (int j = 0; j < players.Length; j++)
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
            if (thisPlayerBase.GetState() == CharacterBase.playerState.Attacking)
            {
                PlayerAttack thisPlayerAttack = other.gameObject.GetComponent<PlayerAttack>();
                thisPlayerAttack.CancelAttack();
            }
            thisPlayerBase.SetState(CharacterBase.playerState.Idle);
            thisPlayerBase.SetSpawnPos(spawnPoint.transform.position);
            thisPlayerBase.SetState(CharacterBase.playerState.Dead);
            //other.gameObject.transform.position = spawnPoint.transform.position;
        }
    }
}
