using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public GameObject pickupEffect;

    private MinigameManager thisManager;

    // Start is called before the first frame update
    void Start()
    {
        thisManager = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<MinigameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            thisManager.HandleItemPickup(gameObject, other.gameObject);
            Instantiate(pickupEffect, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
