using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public GameObject pickupEffect;

    private MinigameManager thisManager;
    private AudioSource ap;

    // Start is called before the first frame update
    void Start()
    {
        thisManager = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<MinigameManager>();
        ap = gameObject.GetComponent<AudioSource>();
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
            ap.Play();
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = false;
            }
            GetComponent<Collider>().enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            Instantiate(pickupEffect, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject, ap.clip.length);
        }
    }
}
