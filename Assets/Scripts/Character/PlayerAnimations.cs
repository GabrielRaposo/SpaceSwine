using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    Animator animator;
    string currentState;

    [HideInInspector] public bool holding;
    [HideInInspector] public float horizontalInput;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update() 
    {
        if (Time.timeScale < 1)
            return;

        if (horizontalInput == 0)
        {
            ChangeAnimationState(!holding ? AnimationState.IDLE : AnimationState.IDLE_HOLD);
        }
        else
        {
            ChangeAnimationState(!holding ? AnimationState.WALK : AnimationState.WALK_HOLD);
        }
    }

    public void ChangeAnimationState (string newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        if (!animator)
        {
            Debug.Log("Player - Animator Fail");
            return;
        }

        animator.Play(newState);
    }
}

public class AnimationState
{
    public static string IDLE = "Player-Idle";
    public static string WALK = "Player-Walk";

        public static string JUMP = "Player-Jump";
        public static string LAUNCH = "Player-Launch";
        public static string FALL = "Player-Fall";

    public static string IDLE_HOLD = "Player-Idle-Hold";
    public static string WALK_HOLD = "Player-Walk-Hold";

        public static string JUMP_HOLD = "Player-Jump-Hold";
        public static string LAUNCH_HOLD = "Player-Launch-Hold";
        public static string FALL_HOLD = "Player-Fall-Hold";
}
