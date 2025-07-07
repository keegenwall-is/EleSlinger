using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    private Camera cam;

    [Header("Camera Settings")]
    public float minFOV;
    public float maxFOV;
    public float zoomLimiter;
    public Vector3 offset;
    public float tightness;
    public float minZOffset;
    public float maxZOffset;

    [Header("Camera Position")]
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayers();
    }

    public void FindPlayers()
    {
        players.Clear();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        players.AddRange(GameObject.FindGameObjectsWithTag("Immune"));
    }

    private void LateUpdate()
    {
        if (players == null || players.Count == 0)
            return;

        MoveCamera();
        AdjustZoom();
    }

    private void MoveCamera()
    {
        Vector3 centerPoint = GetCenterPoint();

        // Determine zoom level
        float fovPercent = Mathf.InverseLerp(minFOV, maxFOV, cam.fieldOfView);

        // Interpolate offset based on zoom level
        offset.z = Mathf.Lerp(minZOffset, maxZOffset, fovPercent);

        // Interpolate clamp bounds
        float adjustedMinX = Mathf.Lerp(minX, 0f, fovPercent);
        float adjustedMaxX = Mathf.Lerp(maxX, 0f, fovPercent);

        // Clamp Z based on camera position = center + offset
        float adjustedMinZ = Mathf.Lerp(minZ, 0f, fovPercent);
        float adjustedMaxZ = Mathf.Lerp(maxZ, 0f, fovPercent);

        // Clamp center
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
        if (players.Count == 1)
            return players[0].transform.position;

        Bounds bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 1; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.center;
    }

    private float GetGreatestDistance()
    {
        Bounds bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 1; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.size.magnitude;
    }
}
