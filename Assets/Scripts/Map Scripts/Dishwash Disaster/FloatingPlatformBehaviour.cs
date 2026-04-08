using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPlatformBehaviour : MonoBehaviour
{
    public Image icon;
    public bool fall = false;
    public float fallSpeed;
    public List<GameObject> onPlayers = new List<GameObject>();

    private List<GameObject> jumpingPlayers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fall)
        {
            transform.parent.position -= Vector3.up * fallSpeed * Time.deltaTime;
            fallSpeed += 10f * Time.deltaTime;
        }

        if (jumpingPlayers.Count > 0)
        {
            for (int i = jumpingPlayers.Count - 1; i >= 0; i--)
            {
                CharacterBase thisBaseScript = jumpingPlayers[i].GetComponent<CharacterBase>();
                if (thisBaseScript.GetState() != CharacterBase.playerState.Dashing && thisBaseScript.GetState() != CharacterBase.playerState.TakingHit)
                {
                    RaycastHit hit;
                    Vector3 rayStartPos = jumpingPlayers[i].transform.position;
                    rayStartPos.y += 3f;

                    Debug.DrawRay(rayStartPos, -jumpingPlayers[i].transform.up * 5f, Color.red, 2);
                    if (Physics.SphereCast(rayStartPos, 2f,  -jumpingPlayers[i].transform.up, out hit, 4f))
                    {
                        if (!hit.collider.name.Contains("Platform"))
                        {
                            thisBaseScript.SetState(CharacterBase.playerState.Falling);
                        }
                    }
                    else
                    {
                        thisBaseScript.SetState(CharacterBase.playerState.Falling);
                    }
                    jumpingPlayers.RemoveAt(i);
                }
            }
        }
    }

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CharacterBase baseScript = other.gameObject.GetComponent<CharacterBase>();
            if (baseScript.GetState() == CharacterBase.playerState.Falling || baseScript.GetState() != CharacterBase.playerState.TakingHit)
            {
                PlayerFall fallScript = other.gameObject.GetComponent<PlayerFall>();
                fallScript.fallSpeed += 20f;
            }
            onPlayers.Add(other.gameObject);
        }
        else if (other.gameObject.name.Contains("Water"))
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CharacterBase baseScript = other.gameObject.GetComponent<CharacterBase>();
            if (baseScript.GetState() != CharacterBase.playerState.Dashing && baseScript.GetState() != CharacterBase.playerState.TakingHit)
            {
                baseScript.SetState(CharacterBase.playerState.Falling);
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
}