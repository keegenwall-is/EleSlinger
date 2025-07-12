using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterBase: MonoBehaviour
{

    private playerState currentState;
    private CapsuleCollider cc;
    private Vector3 spawnPos;
    private PlayerMove playerMove;
    private GameObject thisKO;
    private GameObject jail;

    public InputDevice thisController;
    public Animator anim;
    public Image face;
    public Sprite normalFace;
    public Sprite attackFace;
    public Sprite hitFace;
    public bool isAttacking = false;
    public bool canMove = true;
    public float animFadeDur;
    public float respawnTime = 3.0f;
    public float graceLength = 2.0f;
    public GameObject mesh;
    public GameObject KO;
    public Material[] materials;

    private AnimationClip[] clips;

    public enum playerState
    {
        Idle,
        Running,
        Attacking,
        Melee,
        Dashing,
        TakingHit,
        Stunned,
        Dead,
        Out
    }

    void Awake()
    {
        thisController = Keyboard.current;
        anim = GetComponent<Animator>();
        currentState = playerState.Idle;
        clips = anim.runtimeAnimatorController.animationClips;
        cc = GetComponent<CapsuleCollider>();
        playerMove = GetComponent<PlayerMove>();
        jail = GameObject.FindGameObjectWithTag("Jail");
    }

    public void SetController(InputDevice device)
    {
        thisController = device;
    }

    public void SetState(playerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStateEnter(newState);
        }
    }

    public void SetSpawnPos(Vector3 spawnPos)
    {
        this.spawnPos = spawnPos;
    }

    public void SetMaterial(string matName)
    {
        foreach (Material mat in materials){
            if (mat.name.Contains(matName))
            {
                mesh.GetComponent<Renderer>().material = mat;
            }
        }
    }

    public playerState GetState()
    {
        return currentState;
    }

    public void OnStateEnter(playerState state)
    {
        switch (state)
        {
            case playerState.Idle:
                anim.CrossFade(FindAnimation("Idle"), animFadeDur);
                face.sprite = normalFace;
                canMove = true;
                break;
            case playerState.Running:
                anim.CrossFade(FindAnimation("Run"), animFadeDur);
                face.sprite = normalFace;
                canMove = true;
                break;
            case playerState.Attacking:
                anim.CrossFade(FindAnimation("Attack"), animFadeDur);
                face.sprite = attackFace;
                canMove = false;
                break;
            case playerState.Melee:
                anim.CrossFade(FindAnimation("Selected"), animFadeDur - 0.1f);
                canMove = false;
                break;
            case playerState.Dashing:
                anim.CrossFade(FindAnimation("Jump"), animFadeDur);
                face.sprite = normalFace;
                canMove = true;
                break;
            case playerState.TakingHit:
                anim.CrossFade(FindAnimation("TakeHit"), animFadeDur);
                face.sprite = hitFace;
                canMove = false;
                break;
            case playerState.Stunned:
                face.sprite = hitFace;
                canMove = false;
                break;
            case playerState.Dead:
                anim.CrossFade(FindAnimation("Idle"), animFadeDur);
                face.sprite = hitFace;
                StartCoroutine(Respawn());
                thisKO = Instantiate(KO, transform.position, transform.rotation);
                break;
            case playerState.Out:
                playerMove.rb.velocity = Vector3.zero;
                playerMove.enabled = false;
                cc.enabled = false;
                mesh.SetActive(false);
                thisKO = Instantiate(KO, transform.position, transform.rotation);
                transform.position = jail.transform.position;
                gameObject.tag = "Out";
                CameraMovement camMoveScript = Camera.main.GetComponent<CameraMovement>();
                camMoveScript.FindPlayers();
                break;
        }
    }

    private string FindAnimation(string seg)
    {
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains(seg))
            {
                return clip.name;
            }
        }
        return null;
    }

    IEnumerator Respawn()
    {
        playerMove.enabled = false;
        cc.enabled = false;
        mesh.SetActive(false);
        gameObject.tag = "Immune";

        float elapsed = 0f;
        float dist = Vector3.Distance(transform.position, spawnPos);

        while (elapsed < respawnTime)
        {
            transform.position = Vector3.Lerp(transform.position, spawnPos, elapsed/dist);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = spawnPos;

        yield return new WaitForSeconds(0.1f);

        Destroy(thisKO);
        playerMove.enabled = true;
        SetState(playerState.Idle);
        mesh.SetActive(true);
        cc.enabled = true;

        for (int i = 0; i < graceLength * 2; i++)
        {
            yield return new WaitForSeconds(0.25f);
            mesh.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            mesh.SetActive(true);
        }

        gameObject.tag = "Player";
    }
}
