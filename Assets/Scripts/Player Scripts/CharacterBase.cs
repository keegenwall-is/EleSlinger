using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterBase: MonoBehaviour
{

    private playerState currentState;

    public InputDevice thisController;
    public Animator anim;
    public Image face;
    public Sprite normalFace;
    public Sprite attackFace;
    public Sprite hitFace;
    public bool isAttacking = false;
    public bool canMove = true;
    public float animFadeDur;

    private AnimationClip[] clips;

    public enum playerState
    {
        Idle,
        Running,
        Attacking,
        Dashing,
        TakingHit,
        Blocking,
        Dead
    }

    void Awake()
    {
        thisController = Keyboard.current;
        anim = GetComponent<Animator>();
        currentState = playerState.Idle;
        clips = anim.runtimeAnimatorController.animationClips;
    }

    public void SetState(playerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStateEnter(newState);
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
}
