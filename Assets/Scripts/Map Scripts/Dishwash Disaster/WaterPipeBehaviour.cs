using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPipeBehaviour : MonoBehaviour
{
    public float waterPower;

    private DishwashManager managerScript;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            TakeHit takeHitScript = other.GetComponent<TakeHit>();
            takeHitScript.SetDirection(transform.forward);
            takeHitScript.SetProjPower(waterPower);
            takeHitScript.HitReaction();
        }
    }
}
