﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    Animator animator;
    string currentState;

    [HideInInspector] public bool landedOnGround;
    [HideInInspector] public bool holding;
    [HideInInspector] public bool throwing;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalSpeed;

    enum State  { Landed, Jumping, Flying }
    State state;


    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable() 
    {
        holding = throwing = false;  
    }

    private void Update() 
    {
        if (Time.timeScale < 1)
            return;

        switch (state)
        {
            case State.Landed:
                if (throwing)
                {
                    ChangeAnimationState(AnimationState.THROW_GROUND);
                }
                else if (!landedOnGround)
                {
                    ChangeAnimationState(!holding ? AnimationState.FALL : AnimationState.FALL_HOLD );
                } 
                else if (horizontalInput == 0)
                {
                    ChangeAnimationState(!holding ? AnimationState.IDLE : AnimationState.IDLE_HOLD );
                }
                else
                {
                    ChangeAnimationState(!holding ? AnimationState.WALK : AnimationState.WALK_HOLD );
                }            
                break;

            case State.Jumping:
                if (verticalSpeed > 0)
                {
                    ChangeAnimationState(!holding ? AnimationState.JUMP : AnimationState.JUMP_HOLD );
                }
                else
                {
                    ChangeAnimationState(!holding ? AnimationState.FALL : AnimationState.FALL_HOLD );
                }
                break;

            case State.Flying:
                ChangeAnimationState( !holding ? AnimationState.LAUNCH : AnimationState.LAUNCH_HOLD );
                break;
        }
    }

    public void SetLandedState()
    {
        state = State.Landed;
    }

    public void SetJumpingState()
    {
        ChangeAnimationState( !holding ? AnimationState.JUMP : AnimationState.JUMP_HOLD );
        state = State.Jumping;
    }

    public void SetLaunchedState()
    {
        ChangeAnimationState( !holding ? AnimationState.LAUNCH : AnimationState.LAUNCH_HOLD );
        state = State.Flying;
    }

    public void ChangeAnimationState (string newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        if (!animator)
        {
            Debug.Log("No animator found. ");
            return;
        }

        animator.Play(newState);
    }

    public void AnimationEnd_GroundThrow()
    {
        throwing = false;
    }
}

public class AnimationState
{
    public static string IDLE = "Player-Idle";
    public static string WALK = "Player-Walk";

    public static string LAUNCH = "Player-Launch";
    public static string JUMP = "Player-Jump";
    public static string FALL = "Player-Fall";

    public static string IDLE_HOLD = "Player-Idle-Hold";
    public static string WALK_HOLD = "Player-Walk-Hold";

    public static string LAUNCH_HOLD = "Player-Launch-Hold";
    public static string JUMP_HOLD = "Player-Jump-Hold";
    public static string FALL_HOLD = "Player-Fall-Hold";

    public static string THROW_GROUND = "Player-Throw-Ground";
    //public static string THROW_AIR = "Player-Throw-Air";
}
