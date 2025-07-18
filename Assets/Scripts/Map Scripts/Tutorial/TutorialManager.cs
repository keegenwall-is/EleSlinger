using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MinigameManager
{

    public GameObject[] buttons;
    public List<Text> readyTxts = new List<Text>();

    private bool[] oks = { false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnInteractiveObjectEvent(GameObject obj, GameObject player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (buttons[i] == obj && players[i] == player && oks[i] == false)
            {
                Animator buttonAnim = buttons[i].GetComponent<Animator>();
                buttonAnim.Play("ButtonTurningOn");
                readyTxts[i].text = "OK";
                oks[i] = true;
            }
        }

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
            gameController.LoadRandomMinigame();
        }
    }
}
