using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBehaviour : MonoBehaviour
{

    public float glassSpeed;

    private DishwashManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("Minigame Manager").GetComponent<DishwashManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y > manager.plateHeight + 3.5)
        {
            gameObject.transform.position -= Vector3.up * glassSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.tag = "ObImmune";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ObImmune")
        {
            other.gameObject.tag = "Player";
        }
    }
}
