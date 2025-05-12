using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBehaviour : MonoBehaviour
{

    public float destroyAfterSeconds;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Delete());
    }

    public IEnumerator Delete()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }
}
