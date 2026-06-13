using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoakerBehaviour : MonoBehaviour
{
    public GameObject foam;
    public GameObject splash;

    public float maxDistance = 10f;
    [Header("Cone Dimensions")]
    [Range(0f, 89f)] public float maxConeAngle = 30f; // Outer radius angle of the circle base
    public int totalRings = 3;                       // Layers from center to outer edge
    public int raysPerRing = 8;

    private GameObject player;
    private CharacterBase baseScript;
    private bool foamOn;

    // Start is called before the first frame update
    void Start()
    {
        foamOn = false;
        player = transform.root.gameObject;
        baseScript = player.GetComponent<CharacterBase>();

        if (baseScript.GetState() == CharacterBase.playerState.UsingItem)
        {
            StartCoroutine(StartFiring());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (foamOn)
        {
            FireCircularConeRays();
        }
    }

    private IEnumerator StartFiring()
    {
        yield return new WaitForSeconds(0.2f);
        foam.SetActive(true);
        foamOn = true;
    }

    void FireCircularConeRays()
    {
        Vector3 startPos = transform.position;
        startPos.x += 1f;
        Vector3 origin = transform.position;

        // Define your new target direction (straight down relative to the item)
        Vector3 targetDirection = -transform.up;

        // Create a base rotation that faces our target direction
        Quaternion coneTargetRotation = Quaternion.LookRotation(targetDirection);

        // 1. Core Center Ray
        FireSingleRay(origin, targetDirection);

        // 2. Loop through concentric circular rings
        for (int ring = 1; ring <= totalRings; ring++)
        {
            float currentAngleFraction = ((float)ring / totalRings) * maxConeAngle;

            for (int i = 0; i < raysPerRing; i++)
            {
                float circumferenceProgress = ((float)i / raysPerRing) * 360f * Mathf.Deg2Rad;

                float xOffset = Mathf.Cos(circumferenceProgress);
                float yOffset = Mathf.Sin(circumferenceProgress);

                // 1. Calculate the cone tilt relative to absolute forward (Z-axis)
                Quaternion tiltRotation = Quaternion.Euler(
                    xOffset * currentAngleFraction,
                    yOffset * currentAngleFraction,
                    0
                );

                // 2. Combine them: Start at Z-forward, tilt it into a cone, then point the whole cone DOWN
                Vector3 rayDirection = coneTargetRotation * tiltRotation * Vector3.forward;

                FireSingleRay(origin, rayDirection);
            }
        }
    }

    void FireSingleRay(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        // Fires the ray (Requires 'Queries Hit Triggers' enabled in your Physics settings)
        if (Physics.Raycast(origin, direction, out hit, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            Instantiate(splash, hit.point, Quaternion.identity);
        }

        // Draw the circular cone in the Scene view for testing
        Debug.DrawRay(origin, direction * maxDistance, Color.cyan, 0.1f);
    }
}
