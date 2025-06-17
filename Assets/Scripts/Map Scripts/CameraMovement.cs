using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject[] players;
    private Camera cam;

    [Header("Camera Settings")]
    public float minFOV;
    public float maxFOV;
    public float zoomLimiter;
    public Vector3 offset;
    public float tightness;
    public float minZOffset;
    public float maxZOffset;

    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    private void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayers();
    }

    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void LateUpdate()
    {
        if (players == null || players.Length == 0)
            return;

        MoveCamera();
        AdjustZoom();
    }

    private void MoveCamera()
    {
        Vector3 centerPoint = GetCenterPoint();

        // Adjust offset.z based on FOV
        offset.z = Mathf.Lerp(minZOffset, maxZOffset, Mathf.InverseLerp(minFOV, maxFOV, cam.fieldOfView));

        // Adjust how much we clamp the camera based on FOV
        float fovPercent = Mathf.InverseLerp(minFOV, maxFOV, cam.fieldOfView);

        // Interpolate clamp bounds
        float adjustedMinX = Mathf.Lerp(-55f, 0f, fovPercent);
        float adjustedMaxX = Mathf.Lerp(55f, 0f, fovPercent);

        // Clamp Z based on camera position = center + offset
        float adjustedMinZ = Mathf.Lerp(-70f - offset.z, 0f, fovPercent);
        float adjustedMaxZ = Mathf.Lerp(-5f - offset.z, 0f, fovPercent);

        // Clamp the camera’s center position
        float clampedX = Mathf.Clamp(centerPoint.x, adjustedMinX, adjustedMaxX);
        float clampedZ = Mathf.Clamp(centerPoint.z, adjustedMinZ, adjustedMaxZ);

        Vector3 clampedCenter = new Vector3(clampedX, centerPoint.y, clampedZ);

        transform.position = clampedCenter + offset;
        transform.LookAt(clampedCenter);
    }

    private void AdjustZoom()
    {
        float maxDistance = GetGreatestDistance() * tightness;
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, maxDistance / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 3f);
    }

    private Vector3 GetCenterPoint()
    {
        if (players.Length == 1)
            return players[0].transform.position;

        Bounds bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 1; i < players.Length; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.center;
    }

    private float GetGreatestDistance()
    {
        Bounds bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 1; i < players.Length; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.size.magnitude;
    }
}
