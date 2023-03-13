using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TerminalGenericCall : MonoBehaviour, ITerminalEvent
{
    public UnityEvent action;
    
    // public GameObject parent;
    // [SerializeField] private List<MonoBehaviour> monoBehaviours;
    // [SerializeField] private int selectionIndex;

    public void Activate(InteractableTerminal terminal, PlayerInteractor interactor)
    {
        if(action!=null)
            action.Invoke();
    }
}
