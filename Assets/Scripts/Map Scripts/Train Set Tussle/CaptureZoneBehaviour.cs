using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureZoneBehaviour : MonoBehaviour
{
    public float timeToCapture;
    public GameObject spikes;
    public Animator anim;
    public GameObject firework;

    private List<GameObject> capturingPlayers = new List<GameObject>();
    private float timeToCaptureCurrent;
    private TrainSetManager managerScript;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<TrainSetManager>();
    }

    // Update is called once per frame
    void Update()
    {
        timeToCaptureCurrent += Time.deltaTime;

        if (timeToCaptureCurrent >= timeToCapture)
        {
            timeToCaptureCurrent = 0f;
            foreach (GameObject player in capturingPlayers)
            {
                managerScript.ZoneCaptured(player);
            }
            Vector3 fireworkSpawnPos = transform.position;
            fireworkSpawnPos.y += 10f;
            Instantiate(firework, fireworkSpawnPos, Quaternion.identity);

            Destroy(gameObject);
        }

        for (int i = capturingPlayers.Count - 1; i >= 0; i--)
        {
            GameObject capturingPlayer = capturingPlayers[i];
            CharacterBase baseScript = capturingPlayer.GetComponent<CharacterBase>();

            if (baseScript.GetState() == CharacterBase.playerState.Dead)
            {
                capturingPlayers.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            capturingPlayers.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (capturingPlayers.Contains(other.gameObject))
            {
                capturingPlayers.Remove(other.gameObject);
            }
        }
    }
}
