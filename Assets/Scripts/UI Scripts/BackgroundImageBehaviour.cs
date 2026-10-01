using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageBehaviour : MonoBehaviour
{

    private RawImage rawImage;
    private Vector2 scrollSpeed = new Vector2(0.05f, 0.05f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rawImage = GetComponent<RawImage>();

        float randAngle = Random.Range(0, Mathf.PI * 2f);
        float speedMagnitude = scrollSpeed.magnitude;
        scrollSpeed = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle)) * speedMagnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Rect currentUV = rawImage.uvRect;
        currentUV.position += scrollSpeed * Time.deltaTime;
        rawImage.uvRect = currentUV;
    }
}
