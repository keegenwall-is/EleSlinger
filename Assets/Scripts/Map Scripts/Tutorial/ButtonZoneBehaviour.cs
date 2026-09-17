using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonZoneBehaviour : MonoBehaviour
{
    public bool finalZone;

    private TutorialManager managerScript;

    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<TutorialManager>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (finalZone)
            {
                managerScript.SetOk(managerScript.players.IndexOf(other.gameObject), true);
            }
            else
            {
                managerScript.inPlace[managerScript.players.IndexOf(other.gameObject)] = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (finalZone)
            {
                managerScript.SetOk(managerScript.players.IndexOf(other.gameObject), false);
            }
            else
            {
                managerScript.inPlace[managerScript.players.IndexOf(other.gameObject)] = false;
            }
        }
    }
}
