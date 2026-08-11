using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSetManager : MinigameManager
{
    [Header("Train Set Variables")]
    public GameObject blockGroup;
    public float environmentSpawnCD = 1f;
    public float environmentSpeed;
    public GameObject rail;
    public float bulletSpawnCD = 1f;
    public GameObject[] bullets;

    private float environmentSpawnCurrent;
    public List<GameObject> environmentObjects = new List<GameObject>();
    private float railSpawnCD;
    private float railSpawnCurrent;
    private float railLength = 29.5f;
    private float bulletSpawnCurrent;

    // Start is called before the first frame update
    void Start()
    {
        bulletSpawnCD /= playerNo;
        railSpawnCD = railLength / environmentSpeed;

        for (float i = 200f; i >= -200f; i -= railLength)
        {
            Vector3 spawnPos = new Vector3(i, -10f, 0f);
            GameObject thisRail = Instantiate(rail, spawnPos, Quaternion.identity);
            environmentObjects.Add(thisRail);
        }
    }

    protected override void OnTick()
    {
        environmentSpawnCurrent += Time.deltaTime;

        if (environmentSpawnCurrent >= environmentSpawnCD)
        {
            environmentSpawnCurrent = 0f;
            int updown = Random.Range(0, 2);
            float randomZ = 0f;
            if (updown == 0)
            {
                randomZ = Random.Range(17, 80);
            }
            else
            {
                randomZ = Random.Range(-17, -80);
            }
            Vector3 spawnPos = new Vector3(200f, -10f, randomZ);
            GameObject thisEnvObj = Instantiate(blockGroup, spawnPos, Quaternion.identity);
            environmentObjects.Add(thisEnvObj);
        }

        railSpawnCurrent += Time.deltaTime;

        if (railSpawnCurrent >= railSpawnCD)
        {
            railSpawnCurrent = 0f;
            railSpawnCD = railLength / environmentSpeed;
            Vector3 spawnPos = new Vector3(200f, -10f, 0f);
            GameObject thisRail = Instantiate(rail, spawnPos, Quaternion.identity);
            environmentObjects.Add(thisRail);
        }

        bulletSpawnCurrent += Time.deltaTime;

        if (bulletSpawnCurrent >= bulletSpawnCD)
        {
            int updown = Random.Range(0, 2);
            float randomZ = 0f;
            if (updown == 0)
            {
                randomZ = 200f;
            }
            else
            {
                randomZ = -200f;
            }

            bulletSpawnCurrent = 0f;
            float randomX = Random.Range(-100f, 150f);
            Vector3 spawnPos = new Vector3(randomX, 3f, randomZ);
            int randBullet = Random.Range(0, 6);
            int bulletNo = 0;
            if (randBullet == 0)
            {
                bulletNo = 1;
            }
            GameObject thisBullet = Instantiate(bullets[bulletNo], spawnPos, Quaternion.identity);
            FoamBulletBehaviour bulletScript = thisBullet.GetComponent<FoamBulletBehaviour>();
            int randomTarget = Random.Range(0, players.Count);
            bulletScript.target = players[randomTarget];
            //environmentObjects.Add(thisBullet);
        }

        for (int i = environmentObjects.Count - 1; i >= 0; i--)
        {
            if (environmentObjects[i] == null)
            {
                continue;
            }

            environmentObjects[i].transform.Translate(-Vector3.right * environmentSpeed * Time.deltaTime);

            if (environmentObjects[i].transform.position.x <= -200)
            {
                Destroy(environmentObjects[i]);
                environmentObjects.RemoveAt(i);
            }
        }
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        GameObject spawn = SetPlayerSpawn(player);
        KillPlayer(player, spawn);
    }

    public IEnumerator RemoveFromEnvironment(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        environmentObjects.Remove(obj);
    }
}
