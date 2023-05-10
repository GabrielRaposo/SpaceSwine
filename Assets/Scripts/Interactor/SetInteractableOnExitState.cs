using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Interactable) )]
public class SetInteractableOnExitState : StoryEventDependent
{
    Interactable interactable;

    NavigationConsole navigationConsole;

    private void Awake() 
    {
        interactable = GetComponent<Interactable>();    
    }

    void Start()
    {
        CallDependentAction ( () => UpdateInteractionCheck() );

        NavigationConsole navigationConsole = NavigationConsole.Instance;
        if (navigationConsole)
            navigationConsole.OnStateChange += UpdateInteractionCheck;
    }

    private void UpdateInteractionCheck( bool value = false )
    {
        interactable.SetInteraction( !string.IsNullOrEmpty(ShipInitializerSystem.ExitPath) );       
    }

    private void OnDisable() 
    {
        if (navigationConsole)
            navigationConsole.OnStateChange -= UpdateInteractionCheck;  
    }
}
