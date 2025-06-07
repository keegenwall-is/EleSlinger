using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCurrents : MonoBehaviour
{

    public float chordCD;
    public int numOfCurrents;

    private GameObject[] currents;
    private float chordCurrent = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        currents = GameObject.FindGameObjectsWithTag("Current");
        SwitchCurrents();
    }

    // Update is called once per frame
    void Update()
    {
        chordCurrent += Time.deltaTime;

        if (chordCurrent >= chordCD)
        {
            SwitchCurrents();
            chordCurrent = 0f;
        }
    }

    private void SwitchCurrents()
    {
        for (int i = 0; i < currents.Length; i++)
        {
            currents[i].SetActive(false);
        }

        for (int i = 0; i < numOfCurrents; i++)
        {
            int random = Random.Range(0, currents.Length);
            if (!currents[random].activeSelf)
            {
                currents[random].SetActive(true);
                CapsuleCollider cc = currents[random].GetComponent<CapsuleCollider>();
                StartCoroutine(TurnOnCollider(cc));
            }
            else
            {
                i--;
            }
        }
    }

    private IEnumerator TurnOnCollider(CapsuleCollider cc)
    {
        cc.enabled = false;
        yield return new WaitForSeconds(1f);
        cc.enabled = true;
    }
}
