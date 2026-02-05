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

        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.GetComponent<SphereCollider>().enabled = true;
        }

        mr.material = elecMat;
        yield return new WaitForSeconds(elecTime);
        mr.material = baseMat;
        rTime = Random.Range(minWait, maxWait);
        chordCD = true;

        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }
}
