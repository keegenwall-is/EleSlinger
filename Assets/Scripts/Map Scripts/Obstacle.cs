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
            //If this obstacle has a parent who is immune (i.e. a player with an obstacle attached to them);
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
