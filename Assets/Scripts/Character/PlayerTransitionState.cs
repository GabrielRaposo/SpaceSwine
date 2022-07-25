using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTransitionState : MonoBehaviour
{
    PlayerCharacter playerCharacter;
    PlayerAnimations playerAnimations;

    UnityAction OnAnimationEnd;

    void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    // -- Nome estranho pra ser mais fácil de achar no menu de Animation Event
    public void _Transition_CallEvent () 
    {
        GameManager.BlockCharacterInput = false;

        if (OnAnimationEnd != null)
            OnAnimationEnd.Invoke();
    }

    public void TeleportOut (UnityAction action)
    {
        playerCharacter.DisableInteractions();
        playerAnimations.SetTransitionState( AnimationState.TRANSITION_TELEPORT_OUT );

        OnAnimationEnd = action;

        GameManager.BlockCharacterInput = true;
    }
}
