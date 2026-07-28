using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPlatformBehaviour : MonoBehaviour
{
    public float fallSpeed;
    public List<GameObject> onPlayers = new List<GameObject>();
    public GameObject ring;
    public Image ringImg;
    public Sprite[] ringColours;
    public bool beingSoaked = false;
    public float maxSoaked;
    public GameObject completeVFX;
    public GameObject bubble;
    public bool isBig;
    public bool isStationary;

    private List<GameObject> jumpingPlayers = new List<GameObject>();
    private DishwashManager managerScript;
    private bool canChangePlayer = false;
    private float currentSoaked = 0f;
    private bool isSoaked = false;
    private GameObject capturingPlayer;
    private float currentBubble = 0f;
    private float bubbleCD = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<DishwashManager>();

        if (isBig)
        {
            bubbleCD = 0.05f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpingPlayers.Count > 0)
        {
            for (int i = jumpingPlayers.Count - 1; i >= 0; i--)
            {
                CharacterBase thisBaseScript = jumpingPlayers[i].GetComponent<CharacterBase>();
                PlayerFall PFScript = jumpingPlayers[i].GetComponent<PlayerFall>();
                if (thisBaseScript.GetState() != CharacterBase.playerState.Dashing && thisBaseScript.GetState() != CharacterBase.playerState.TakingHit)
                {
                    RaycastHit hit;
                    Vector3 rayStartPos = jumpingPlayers[i].transform.position;
                    rayStartPos.y += 3f;

                    Debug.DrawRay(rayStartPos, -jumpingPlayers[i].transform.up * 5f, Color.red, 2);
                    if (Physics.SphereCast(rayStartPos, 1f,  -jumpingPlayers[i].transform.up, out hit, 4f))
                    {
                        if (!hit.collider.name.Contains("Platform"))
                        {
                            PFScript.StartFall();
                        }
                    }
                    else
                    {
                        PFScript.StartFall();
                    }
                    jumpingPlayers.RemoveAt(i);
                }
            }
        }

        for (int i = onPlayers.Count - 1; i >= 0; i--)
        {
            GameObject onPlayer = onPlayers[i];
            CharacterBase baseScript = onPlayer.GetComponent<CharacterBase>();

            if (baseScript.GetState() != CharacterBase.playerState.Dashing && !isStationary)
            {
                baseScript.FollowFloatingPlatforms(managerScript.plateSpeed);
            }

            if (baseScript.GetState() == CharacterBase.playerState.Dead)
            {
                onPlayers.RemoveAt(i);
            }
        }

        if (beingSoaked && !isSoaked)
        {
            currentSoaked += Time.deltaTime;
            ringImg.fillAmount = currentSoaked / maxSoaked;
            if (currentSoaked >= maxSoaked)
            {
                ringImg.fillAmount = 1f;
                isSoaked = true;
                managerScript.IncreaseScoreFor(capturingPlayer, isBig);
                completeVFX.SetActive(true);
            }
        }
        else if (isSoaked)
        {
            currentBubble += Time.deltaTime;

            if (currentBubble >= bubbleCD)
            {
                Vector3 randTrans = transform.position;
                if (isBig)
                {
                    randTrans.x += Random.Range(-20f, 20f);
                    randTrans.z += Random.Range(-20f, 20f);
                }
                else
                {
                    randTrans.x += Random.Range(-5f, 5f);
                    randTrans.z += Random.Range(-5f, 5f);
                }

                Instantiate(bubble, randTrans, Quaternion.identity);
                currentBubble = 0f;
            }
        }

        beingSoaked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CharacterBase baseScript = other.gameObject.GetComponent<CharacterBase>();
            if (baseScript.GetState() == CharacterBase.playerState.Falling || baseScript.GetState() == CharacterBase.playerState.TakingHit)
            {
                PlayerFall fallScript = other.gameObject.GetComponent<PlayerFall>();
                fallScript.fallDown = true;
            }
            onPlayers.Add(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CharacterBase baseScript = other.gameObject.GetComponent<CharacterBase>();
            PlayerFall fallScript = other.gameObject.GetComponent<PlayerFall>();
            if (baseScript.GetState() != CharacterBase.playerState.Dashing && baseScript.GetState() != CharacterBase.playerState.TakingHit
                && baseScript.GetState() != CharacterBase.playerState.Running && baseScript.GetState() != CharacterBase.playerState.UsingItem)
            {
                fallScript.StartFall();
            }
            else
            {
                jumpingPlayers.Add(other.gameObject);
            }

            for (int i = onPlayers.Count - 1; i >= 0; i--)
            {
                if (other.gameObject == onPlayers[i])
                {
                    onPlayers.RemoveAt(i);
                }
            }
        }
    }

    public void UpdateCapturingPlayer(int playerNo, GameObject player)
    {
        if (isSoaked)
        {
            return;
        }

        beingSoaked = true;

        if (ring.activeSelf == false)
        {
            ring.SetActive(true);
            ringImg.sprite = ringColours[playerNo];
            capturingPlayer = player;
            StartCoroutine(CanChangePlayer());
        }
        else if (canChangePlayer)
        {
            ringImg.sprite = ringColours[playerNo];
            capturingPlayer = player;
            StartCoroutine(CanChangePlayer());
        }
    }

    private IEnumerator CanChangePlayer()
    {
        canChangePlayer = false;
        yield return new WaitForSeconds(0.2f);
        canChangePlayer = true;
    }
}