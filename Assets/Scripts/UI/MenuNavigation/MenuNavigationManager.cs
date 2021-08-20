using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuNavigationManager : MonoBehaviour
{
    List<NavigationTab> tabs;
    int current;

    bool active;
    
    PlayerInputActions playerInputActions;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        playerInputActions.UI.Navigation.performed += (ctx) => 
        { 
            Vector2 navigationInput = ctx.ReadValue<Vector2>();

            if (navigationInput.y == 0)
                return;

            if (navigationInput.y > .5f)
                MoveCursor(-1);
            else if (navigationInput.y < .5f)
                MoveCursor(1);
        };
        playerInputActions.UI.Navigation.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        { 
            tabs[current].Submit();
        };
        playerInputActions.UI.Confirm.Enable();
    }

    void Start()
    {
        tabs = new List<NavigationTab>();
        GetComponentsInChildren<NavigationTab>(tabs);
        if (tabs.Count < 1)
        {
            enabled = false;
            return;
        }

        current = 0;
        SelectCurrent();
    }

    private void SelectCurrent()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (i == current)
                tabs[i].Select();
            else
                tabs[i].Deselect();
        }
    }

    private void MoveCursor (int direction)
    {
        current += direction;
        if (current < 0)
            current = tabs.Count - 1;
        current %= tabs.Count;

        SelectCurrent();
    }

    private void OnDisable() 
    {
        playerInputActions.UI.Navigation.Disable();
    }
}
