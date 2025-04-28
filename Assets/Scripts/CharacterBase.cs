using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterBase: MonoBehaviour
{

    private playerState currentState;

    public InputDevice thisController;
    public Animator anim;
    public bool isAttacking = false;
    public bool canMove = true;

    public enum playerState
    {
        Idle,
        Running,
        Attacking,
        Blocking,
        Dashing,
        TakingDamage,
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
                canMove = true;
                break;
            case playerState.Running:
                anim.Play("CatRun");
                canMove = true;
                break;
            case playerState.Attacking:
                anim.Play("CatAttack");
                canMove = false;
                break;
            case playerState.Dashing:
                anim.Play("CatJump");
                canMove = true;
                break;
        }
    }
}
