using UnityEngine;
using UnityEngine.UI;

public class DirectionArrow : MonoBehaviour
{

    public Transform target;
    public Image arrowImg;

    private Vector3 initialScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialScale = arrowImg.rectTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Camera cam = Camera.main;

            // 1. Keep your existing distance scaling
            float distance = Vector3.Distance(transform.position, cam.transform.position);
            float clampedDistance = Mathf.Clamp(distance, 2f, 35f);
            arrowImg.rectTransform.localScale = initialScale * clampedDistance * 0.05f;

            // 2. Get the direction from the arrow to the target
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 3. Convert that world direction into the camera's local 2D space
            Vector3 localDir = cam.transform.InverseTransformDirection(dirToTarget);

            // 4. Calculate the angle on the camera's plane (in degrees)
            float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;

            // 5. Match the camera's rotation, then spin only on the Z-axis
            // Note: Subtract 90f if your arrow sprite points UP by default instead of RIGHT
            transform.rotation = cam.transform.rotation * Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target.transform;
        arrowImg.enabled = true;
    }

    public void ForgetTarget()
    {
        target = null;
        arrowImg.enabled = false;
    }
}
