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
    public bool isAttacking = false;
    public bool canMove = true;

    public enum playerState
    {
        Idle,
        Running,
        Attacking,
        Dashing,
        TakingDamage,
        Blocking,
        Dead
    }

    void Awake()
    {
        thisController = Keyboard.current;
        anim = GetComponent<Animator>();
        currentState = playerState.Idle;
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
                anim.Play("CatIdle");
                face.sprite = normalFace;
                canMove = true;
                break;
            case playerState.Running:
                anim.Play("CatRun");
                face.sprite = normalFace;
                canMove = true;
                break;
            case playerState.Attacking:
                anim.Play("CatAttack");
                face.sprite = attackFace;
                canMove = false;
                break;
            case playerState.Dashing:
                anim.Play("CatJump");
                face.sprite = normalFace;
                canMove = true;
                break;
            case playerState.TakingDamage:
                canMove = false;
                break;
        }
    }
}
