using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switches : MonoBehaviour
{
    public float currentLength;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Current") && anim != null)
        {
            anim.SetBool("Switch On", true);
            StartCoroutine(SwitchOff());
        }
    }

    private IEnumerator SwitchOff()
    {
        yield return new WaitForSeconds(currentLength - 0.5f);
        anim.SetBool("Switch On", false);
    }
}
