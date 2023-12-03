using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GGSMenuManager : StoryEventDependent
{
    [SerializeField] Transform verticalLayout;
    [SerializeField] List<StoryEventScriptableObject> minigameUnlockEvents;    

    bool OnFocus;
    int index = -1;
    List<GGSMenuTab> menuTabs;

    PlayerInputActions inputActions;

    private void OnEnable()
    {
        inputActions = new PlayerInputActions();

        inputActions.UI.Navigation.performed += NavigationInput;
        inputActions.UI.Navigation.Enable();

        inputActions.UI.Confirm.performed += ConfirmInput;
        inputActions.UI.Confirm.Enable();

        inputActions.UI.Other.performed += ConfirmInput;
        inputActions.UI.Other.Enable();

        inputActions.UI.Cancel.performed += CancelInput;
        inputActions.UI.Cancel.Enable();

        inputActions.Enable();
    }

    void Start()
    {
        if (!verticalLayout)
            return;
        
        menuTabs = new List<GGSMenuTab>();
        verticalLayout.GetComponentsInChildren(menuTabs);

        CallDependentAction ( () => UpdateTabsInteractable(true), extraFrames: 1 );

        index = 0;
        UpdateTabsHighlight();

        OnFocus = true; // ---------------------------------------- TEMP
    }

    private void UpdateTabsInteractable(bool aux)
    {
        if (!aux)
            return;

        for (int i = 0; i < menuTabs.Count - 1; i++)
        {
            bool state = i < minigameUnlockEvents.Count && StoryEventsManager.IsComplete(minigameUnlockEvents[i]);
            menuTabs[i].SetInteractableState(state);
        }
        menuTabs[menuTabs.Count - 1].SetInteractableState(true);
    }

    private void UpdateTabsHighlight()
    {
        if (menuTabs.Count < 1)
            return;

        for (int i = 0; i < menuTabs.Count; i++) 
            menuTabs[i].SetHighlightState (i == index);
    }

    private void NavigationInput (InputAction.CallbackContext ctx)
    {
        if (!OnFocus)
            return;

        Vector2 axis = ctx.ReadValue<Vector2>();
        if (axis.y == 0)
            return;

        MoveCursor (axis.y > 0 ? -1 : 1);
    }

    private void MoveCursor (int value)
    {
        index += value;
        if (index < 0)
            index = menuTabs.Count - 1;
        index %= menuTabs.Count;

        UpdateTabsHighlight();
    }

    private void ConfirmInput (InputAction.CallbackContext ctx)
    {
        if (!OnFocus)
            return;

        menuTabs[index % menuTabs.Count].OnSubmit();
    }

    public void OnSubmit() // -- Chamada externa
    {
        Debug.Log("Play Jumper");
    }

    private void CancelInput (InputAction.CallbackContext ctx)
    {
        if (!OnFocus)
            return;

        ExitMenu();
    }

    public void ExitMenu()
    {
        Debug.Log("Exit menu");
    }


    private void OnDisable()
    {
        inputActions.UI.Navigation.Disable();

        inputActions.UI.Confirm.Disable();
        inputActions.UI.Other.Disable();
        inputActions.UI.Cancel.Disable();

        inputActions.Disable();
    }
}
