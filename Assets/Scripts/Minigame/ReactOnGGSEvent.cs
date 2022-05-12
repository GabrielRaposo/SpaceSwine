using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Minigame;

public class ReactOnGGSEvent : MonoBehaviour
{
    public UnityEvent OnTurnOn;
    public UnityEvent OnTurnOff;

    void Start()
    {
        GGSConsole ggs = GGSConsole.Instance;
        if (!ggs)
            return;

        if (OnTurnOn == null)
            OnTurnOn = new UnityEvent();

        if (OnTurnOff == null)
            OnTurnOff = new UnityEvent();

        ggs.OnStateChange += (value) => 
        {
            if (value)
                OnTurnOn.Invoke();
            else
                OnTurnOff.Invoke();

        };
    }
}
