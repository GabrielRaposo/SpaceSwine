using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildEndInputListener : MonoBehaviour
{
    PlayerInputActions inputActions;

    private void OnEnable() 
    {
        inputActions = new PlayerInputActions();

        inputActions.UI.Confirm.performed += (ctx) => 
        {
            GameManager.QuitGame();
        };
        inputActions.UI.Confirm.Enable();
    }

    private void OnDisable() 
    {
        if (inputActions != null)
            return;

        inputActions.UI.Confirm.Disable();
    }
}
