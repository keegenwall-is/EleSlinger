using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBoardBehaviour : MonoBehaviour
{
    public GameObject rollingPin;
    public float pinSpeed;
    public float pinHeight = 6f;

    private bool rollingDown;
    private GameObject thisRollingPin;
    private DishwashManager managerScript;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y += pinHeight;
        thisRollingPin = Instantiate(rollingPin, spawnPos, Quaternion.Euler(0f, 0f, 90f));
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<DishwashManager>();
        rollingDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (thisRollingPin.transform.position.z >= 35f)
        {
            rollingDown = true;
        }
        else if (thisRollingPin.transform.position.z <= -35f)
        {
            rollingDown = false;
        }

        if (rollingDown)
        {
            thisRollingPin.transform.position -= Vector3.forward * pinSpeed * Time.deltaTime;
        }
        else
        {
            thisRollingPin.transform.position += Vector3.forward * pinSpeed * Time.deltaTime;
        }

        thisRollingPin.transform.position -= Vector3.right * managerScript.plateSpeed * Time.deltaTime;
    }
}
