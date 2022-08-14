using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTerminalActivationOnTheFirstTime : MonoBehaviour
{
    static bool FirstTime = true;

    void OnEnable()
    {
        if (FirstTime)
        {
            FirstTime = false;
            return;
        }

        OnStateChange (true);
    }

    private void OnStateChange (bool state)
    {
        Interactable interactable = GetComponentInChildren<Interactable>();
        if (!interactable)
            return;

        interactable.autoActivate = false;
        interactable.SetInteraction(state);
    }
}
