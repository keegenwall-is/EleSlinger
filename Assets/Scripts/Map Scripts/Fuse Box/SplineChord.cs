using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineChord : MonoBehaviour
{
    private float currentT;
    private bool chordCD;
    private float rTime;
    private MeshRenderer mr;
    private Material baseMat;
    private Material warningMat;
    private Material elecMat;
    private float minWait;
    private float maxWait;
    private float elecTime;
    private int noOfFlashes;
    private GameObject electricity;
    private AudioSource ap;
    private AudioClip ac;

    // Start is called before the first frame update
    void Start()
    {
        chordCD = true;
        rTime = Random.Range(minWait, maxWait);
        mr = GetComponent<MeshRenderer>();
    }

    public void SetMats(Material baseMat, Material warningMat, Material elecMat)
    {
        this.baseMat = baseMat;
        this.warningMat = warningMat;
        this.elecMat = elecMat;
    }

    public void SetWait(float minWait, float maxWait, float elecTime)
    {
        this.minWait = minWait;
        this.maxWait = maxWait;
        this.elecTime = elecTime;
    }

    public void SetNoOfFlashes(int noOfFlashes)
    {
        this.noOfFlashes = noOfFlashes;
    }

    public void SetElectricity(GameObject electricity)
    {
        this.electricity = electricity;
        electricity.SetActive(false);
    }

    public void SetSFX(AudioSource ap, AudioClip ac)
    {
        this.ap = ap;
        this.ac = ac;
        ap.clip = this.ac;
        ap.volume = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {   
        if (chordCD)
        {
            currentT += Time.deltaTime;
        }

        if (currentT > rTime)
        {
            chordCD = false;
            currentT = 0f;
            StartCoroutine(ChordWarning());
        }
    }

    private IEnumerator ChordWarning()
    {
        for (int i = 0; i < noOfFlashes; i++)
        {
            mr.material = warningMat;
            yield return new WaitForSeconds(0.25f);
            mr.material = baseMat;
            yield return new WaitForSeconds(0.25f);
        }

        ap.Play();
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject == electricity)
            {
                electricity.SetActive(true);
            }
            else
            {
                child.gameObject.GetComponent<SphereCollider>().enabled = true;
            }
        }

        mr.material = elecMat;
        yield return new WaitForSeconds(elecTime);
        mr.material = baseMat;
        rTime = Random.Range(minWait, maxWait);
        chordCD = true;

        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject == electricity)
            {
                electricity.SetActive(false);
            }
            else
            {
                child.gameObject.GetComponent<SphereCollider>().enabled = false;
            }
        }
    }
}
