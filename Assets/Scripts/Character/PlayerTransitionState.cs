using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTransitionState : MonoBehaviour
{
    PlayerCharacter playerCharacter;
    PlayerInput playerInput;
    PlayerAnimations playerAnimations;

    UnityAction OnAnimationEnd;

    public enum State { None, Teleport }
    public static State EnterState;

    void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    private void Start() 
    {
        Debug.Log("PlayerTransitionState - Start");
        Debug.Log("EnterState: " + EnterState);

        switch (EnterState)
        {
            case State.Teleport:
                
                TeleportIn ( action: () => 
                {
                    Debug.Log("Exit intro state");
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
        playerCharacter.DisableAllInteractions();
        playerAnimations.SetTransitionState( AnimationState.TRANSITION_TELEPORT_OUT );

        OnAnimationEnd = action;

        GameManager.BlockCharacterInput = true;
    }

    public void TeleportIn (UnityAction action)
    {
        Debug.Log("-- TeleportIn();");

        playerCharacter.DisableAllInteractions();
        playerAnimations.SetTransitionState( AnimationState.TRANSITION_TELEPORT_IN );

        //GameManager.BlockCharacterInput = true;

        OnAnimationEnd = action;
    }
}
