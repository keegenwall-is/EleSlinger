using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MinigameManager
{

    public GameObject[] buttons;
    public List<Text> readyTxts = new List<Text>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnInteractiveObjectEvent(GameObject obj)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == obj)
            {
                Animator buttonAnim = buttons[i].GetComponent<Animator>();
                buttonAnim.Play("ButtonTurningOn");
                readyTxts[i].text = "OK";
            }
        }
    }
}
