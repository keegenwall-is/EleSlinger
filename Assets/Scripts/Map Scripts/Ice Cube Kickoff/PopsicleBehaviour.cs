using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopsicleBehaviour : MonoBehaviour
{

    public float duration = 5.0f;

    private float elapsedTime = 0f;
    private Vector3 startPos;
    private Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        int updown = Random.Range(0, 2);
        float randStart = Random.Range(-40f, 40f);
        float randEnd = Random.Range(-40f, 40f);
        if (updown == 0)
        {
            startPos = new Vector3(randStart, 22, 50);
            endPos = new Vector3(randEnd, 22, -50);
        }
        else
        {
            startPos = new Vector3(randStart, 22, -50);
            endPos = new Vector3(randEnd, 22, 50);
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        float percentage = elapsedTime / duration;

        transform.position = Vector3.Lerp(startPos, endPos, percentage);

        if (percentage >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
