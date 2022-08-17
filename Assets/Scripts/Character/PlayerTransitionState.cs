﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTransitionState : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event teleportInAKEvent;
    [SerializeField] AK.Wwise.Event teleportOutAKEvent;

    PlayerCharacter playerCharacter;
    PlayerInput playerInput;
    PlayerAnimations playerAnimations;

    UnityAction OnAnimationEnd;

    public enum State { None, Teleport }
    public static State EnterState;

    public static bool BlockSpawn;

    void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    private void Start() 
    {
        //Debug.Log("PlayerTransitionState - EnterState: " + EnterState);

        switch (EnterState)
        {
            case State.Teleport:
                
                TeleportIn ( action: () => 
                {
                    playerCharacter.BlockSpawnEffects(false);
                    playerAnimations.ExitTransitionState();

                    playerCharacter.SetPhysicsBody(true);
                    playerCharacter.ResetStates();
                });

                break;
        }

        EnterState = State.None;
    }

    // -- NÃO MUDAR O NOME DESSA FUNÇÃO: nome específico pra ser mais fácil de achar no menu de Animation Event
    public void _Transition_CallEvent () 
    {
        GameManager.BlockCharacterInput = false;

        if (OnAnimationEnd != null)
            OnAnimationEnd.Invoke();
    }

    public void TeleportOut (UnityAction action)
    {
        if (teleportOutAKEvent != null)
            teleportOutAKEvent.Post(gameObject);

        playerCharacter.DisableAllInteractions();
        playerAnimations.SetTransitionState( AnimationState.TRANSITION_TELEPORT_OUT );

        OnAnimationEnd = action;

        GameManager.BlockCharacterInput = true;
    }

    public void TeleportIn (UnityAction action)
    {
        playerCharacter.DisableAllInteractions();
        playerCharacter.SetHiddenState(true);
        playerCharacter.BlockSpawnEffects(true);
        playerCharacter.SetPhysicsBody(false);

        StartCoroutine( WaitForBlock(frames: 3, () => 
        {
            if (teleportInAKEvent != null)
                teleportInAKEvent.Post(gameObject);    

            playerCharacter.SetHiddenState(false);
            playerCharacter.SetPhysicsBody(true);

            playerAnimations.SetTransitionState( AnimationState.TRANSITION_TELEPORT_IN );

            OnAnimationEnd = action;
        }));
    }

    private IEnumerator WaitForBlock (int frames, UnityAction action)
    {
        yield return new WaitWhile( () => BlockSpawn );

        for (int i = 0; i < frames; i++)
            yield return new WaitForEndOfFrame();

        action.Invoke();
    }
}
