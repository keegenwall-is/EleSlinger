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
    public float minZPos;
    public float maxZPos;
    public float minXPos;
    public float maxXPos;


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
        offset.z = Mathf.Lerp(minZOffset, maxZOffset, Mathf.InverseLerp(minFOV, maxFOV, cam.fieldOfView));
        transform.position = centerPoint + offset;
        transform.LookAt(centerPoint);
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
