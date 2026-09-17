using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MinigameManager
{

    public GameObject[] buttons;
    public List<Text> readyTxts = new List<Text>();
    public GameObject puff;
    public bool[] inPlace = { false, false, false, false };
    public GameObject spawnPointParent;
    public GameObject horizontalBar;
    public GameObject verticalBar;
    public GameObject[] instructionCanvass;
    public TextMeshProUGUI[] instructionTexts;
    public GameObject[] controlImages;

    private bool[] oks = { false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = spawnPointParent.transform.position;
        startPos.x += 110f;
        spawnPointParent.transform.position = startPos;

        if (playerNo == 1)
        {
            Destroy(verticalBar);
            Destroy(horizontalBar);
        }
        else if (playerNo == 2)
        {
            Destroy(horizontalBar);
        }
    }

    protected override void OnObstacleEvent(GameObject player)
    {
        GameObject spawn = SetPlayerSpawn(player);
        KillPlayer(player, spawn);
    }

    protected override void OnInteractiveObjectEvent(GameObject obj, GameObject player, GameObject other)
    {

        if (obj.name.Contains("Button"))
        {
            if (inPlace[players.IndexOf(player)] == false)
            {
                return;
            }

            Transform[] allChildren = obj.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                Instantiate(puff, child.position, Quaternion.identity);
            }
            Destroy(obj);
        }
    }

    public void SetOk(int playerNo, bool isReady)
    {
        if (isReady)
        {
            readyTxts[playerNo].text = "OK";
            oks[playerNo] = true;

            bool allReady = true;
            for (int i = 0; i < players.Count; i++)
            {
                if (oks[i] == false)
                {
                    allReady = false;
                }
            }

            if (allReady)
            {
                StartCoroutine(LoadMinigameAfterTime());
            }
        }
        else
        {
            readyTxts[playerNo].text = "X";
            oks[playerNo] = false;
        }
    }

    private IEnumerator LoadMinigameAfterTime()
    {
        yield return new WaitForSeconds(1.0f);

        gameController.LoadRandomMinigame();
    }
}
