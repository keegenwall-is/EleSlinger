using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{

    private MinigameManager managerScript;
    private GameObject thrower;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<MinigameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Proj"))
        {
            AttackBase attackScript = other.gameObject.GetComponent<AttackBase>();
            thrower = attackScript.GetThrower();
            managerScript.TriggerInteractiveObjectEvent(gameObject, thrower);
        }
    }
}
