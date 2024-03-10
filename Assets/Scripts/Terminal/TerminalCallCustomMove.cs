using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CustomMove))]
public class TerminalCallCustomMove : TerminalCustomCaller, ITerminalEvent
{
    CustomMove customMove;

    int calls;

    private void Awake()
    {
        customMove = GetComponent<CustomMove>();    
    }

    void Start()
    {
        customMove.OnStepReached += OnStepReached;
        calls = 0;

        Round round = GetComponentInParent<Round>();
        if (round) round.OnReset += OnReset;
    }

    public void Activate (InteractableTerminal terminal, PlayerInteractor interactor) 
    {    
        Debug.Log("Activate");
        calls++;
        customMove.ResumeMovement();

        SetInteractionAction?.Invoke(false);
    }

    private void OnStepReached()
    {
        calls--;
        customMove.PauseMovement();

        if (calls > 0)
            customMove.ResumeMovement();
        else
            SetInteractionAction?.Invoke(true);
    }

    private void OnReset()
    {
        SetInteractionAction?.Invoke(true);
        calls = 0;
    }
}
