using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentFlow : MonoBehaviour
{

    public GameObject[] currents;
    public float currentCdMin;
    public float currentCdMax;
    public float currentLength;
    public CapsuleCollider[] colliders;
    public Material baseMaterial;
    public Material redMaterial;
    public float blinkTime;
    public int blinkNo;
    public bool decider;

    private float randomCd;
    private float currentCd = 0;
    private bool currentOn = false;
    private bool canBeTriggered = true;
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        randomCd = Random.Range(currentCdMin, currentCdMax);
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentOn)
        {
            currentCd += Time.deltaTime;
        }

        if (currentCd >= randomCd)
        {
            decider = true;
            canBeTriggered = false;
            StartCoroutine(ActivateCurrent());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Chord") && !currentOn && canBeTriggered)
        {
            CurrentFlow flowScript = other.GetComponent<CurrentFlow>();
            if (flowScript.currentOn)
            {
                foreach (GameObject current in currents)
                {
                    canBeTriggered = false;
                    StartCoroutine(ActivateCurrent());
                }
                if (flowScript.decider)
                {
                    decider = false;
                }
            }
        }
    }

    private IEnumerator ActivateCurrent()
    {
        currentCd = 0;
        currentOn = true;

        StartCoroutine(ChordFlash());

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        foreach (GameObject current in currents)
        {
            current.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }

        yield return new WaitForSeconds(currentLength);
        foreach (GameObject current in currents)
        {
            current.SetActive(false);
        }

        currentOn = false;

        yield return new WaitForSeconds(0.1f);

        if (decider)
        {
            randomCd = Random.Range(currentCdMin, currentCdMax);
        }
        else
        {
            // Stops non deciders from ever being the cause of triggering other chord segments
            randomCd = currentCdMax + 1;
        }
        canBeTriggered = true;  
    }

    private IEnumerator ChordFlash()
    {
        for (int i = 0; i < blinkNo; i++)
        {
            rend.material = redMaterial;
            yield return new WaitForSeconds(blinkTime);
            rend.material = baseMaterial;
            yield return new WaitForSeconds(blinkTime);
        }
    }
}
