using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Minigame;

public class InteractableGGSConsole : Interactable
{
    GGSConsole ggsConsole;

    private void Awake() 
    {
        ggsConsole = GGSConsole.Instance;    
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (ggsConsole)
        {
            //ggsConsole.ToggleConsoleState();
        }
    }

    protected override void HighlightState (bool value) { }

    public override void IconState (bool value) { }
}
