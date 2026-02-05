using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class FuseBoxGenerator : MonoBehaviour
{

    public GameObject map;
    public Material chordMat;
    public GameObject[] fuseObjects;
    public Vector3[] positions;
    public LayerMask hitLayers;
    public float startDist;
    public int minChordLength;
    public int maxChordLength;
    public float minWait;
    public float maxWait;
    public float elecTime;
    public int noOfFlashes;
    public Material baseMat;
    public Material warningMat;
    public Material elecMat;
    public GameObject[] itemSpawners;

    public List<Transform> allPoints = new List<Transform>();
    public List<Transform> chordStartPoints = new List<Transform>();
    public List<Transform> chordEndPoints = new List<Transform>();
    private List<GameObject> destroyedEndPoints = new List<GameObject>();

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //For spawning item blocks, give them a collider so that the chords dont spawn on the top of them and then remove them later
        for (int i = fuseObjects.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (fuseObjects[i], fuseObjects[j]) = (fuseObjects[j], fuseObjects[i]);
        }

        for (int i = positions.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject thisbox = Instantiate(fuseObjects[i % fuseObjects.Length], map.transform);
            thisbox.transform.position = positions[i];
            float randY = UnityEngine.Random.Range(0, 2);
            thisbox.transform.rotation = Quaternion.Euler(0, randY * 180, 0);
        }
        
        //Force the objects to be instantiated before using sphere cast
        Physics.SyncTransforms();

        foreach (Transform child in map.transform)
        {
            if (child.tag == "Chord End Point")
            {
                allPoints.Add(child);
            }

            foreach (Transform grandchild in child)
            {
                if (grandchild.CompareTag("Chord End Point"))
                {
                    allPoints.Add(grandchild);
                }
            }
        }

        /* go through all remaining points of all points in a random order
          and if you can raycast to it from 5 out from the start to 5 out from the end
          then make a connection, move start and end to start and end and remove from all points, re randomize*/

        while (allPoints.Count > 0)
        {
            //Randomize allPoints list
            for (int i = allPoints.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (allPoints[i], allPoints[j]) = (allPoints[j], allPoints[i]);
            }

            bool foundMatch = false;

            //Move through the entire list except for the first item
            for (int i = 1; i < allPoints.Count; i++)
            {
                Vector3 startPos = allPoints[0].position + allPoints[0].forward * startDist;
                Vector3 endPos = allPoints[i].position + allPoints[i].forward * startDist;
                Vector3 checkDir = endPos - startPos;
                float dis = checkDir.magnitude;
                if (dis < minChordLength || dis > maxChordLength)
                {
                    continue;
                }
                checkDir.Normalize();

                // if nothing is blocking the sphere cast then we have a match, remove them from all points
                bool hitSomething = Physics.SphereCast(startPos, 0.25f, checkDir, out RaycastHit hit, dis);
                if (!hitSomething)
                {
                    //Debug.DrawLine(startPos + Vector3.up, endPos + Vector3.up, Color.green, 50f);
                    chordStartPoints.Add(allPoints[0]);
                    chordEndPoints.Add(allPoints[i]);
                    allPoints.RemoveAt(i);
                    allPoints.RemoveAt(0);
                    foundMatch = true;
                    break;
                }
            }

            //If there are no matches then just delete the point
            if (!foundMatch)
            {
                allPoints.RemoveAt(0);
            }
        }

        //using EndPoints.count to stop this breaking with an odd number of chords
        for (int i = 0; i < chordEndPoints.Count; i++)
        {
            GameObject spline = new GameObject("RuntimeSpline");
            SplineContainer container = spline.AddComponent<SplineContainer>();

            Vector3 start = chordStartPoints[i].position;
            Vector3 beta = start + chordStartPoints[i].forward * startDist;
            Vector3 end = chordEndPoints[i].position;
            Vector3 penult = end + chordEndPoints[i].forward * startDist;

            container.Spline.Add(new BezierKnot(start));
            container.Spline.Add(new BezierKnot(beta));
            container.Spline.Add(new BezierKnot(penult));
            container.Spline.Add(new BezierKnot(end));

            SplineExtrude extrude = spline.AddComponent<SplineExtrude>();
            extrude.container = container;

            MeshFilter mf = spline.GetComponent<MeshFilter>();
            Mesh runtimeMesh = new Mesh();
            runtimeMesh.name = "GeneratedSplineMesh";
            mf.sharedMesh = runtimeMesh;

            extrude.radius = 0.25f;
            extrude.sides = 8;
            extrude.rebuildOnSplineChange = true;

            MeshRenderer mr = spline.GetComponent<MeshRenderer>();
            mr.material = chordMat;

            SplineChord chordScript = spline.AddComponent<SplineChord>();
            chordScript.SetMats(baseMat, warningMat, elecMat);
            chordScript.SetWait(minWait, maxWait, elecTime);
            chordScript.SetNoOfFlashes(noOfFlashes);

            float splineLength = container.CalculateLength();

            for (int j = 0; j <= maxChordLength; j++)
            {
                float t = j / (float)maxChordLength;
                Vector3 pos = container.EvaluatePosition(t);

                GameObject triggerPoint = new GameObject("SplineTrigger_" + j);
                triggerPoint.transform.parent = spline.transform;
                triggerPoint.transform.position = pos;

                SphereCollider sc = triggerPoint.AddComponent<SphereCollider>();
                sc.isTrigger = true;
                sc.radius = extrude.radius;
                sc.enabled = false;
                triggerPoint.AddComponent<Obstacle>();
            }
        }

        //Spawners start with colliders so that chords will not be spawned over the top of them
        //Need to turn these off before play
        foreach (GameObject spawner in itemSpawners)
        {
            spawner.GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
