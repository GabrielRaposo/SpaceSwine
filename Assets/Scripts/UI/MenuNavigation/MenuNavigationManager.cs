using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuNavigationManager : MonoBehaviour
{
    [SerializeField] bool startOnFocus;

    List<NavigationTab> tabs;
    int current;

    [HideInInspector] public bool OnFocus;
    
    PlayerInputActions playerInputActions;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        playerInputActions.UI.Navigation.performed += (ctx) => 
        { 
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            Vector2 navigationInput = ctx.ReadValue<Vector2>();

            if (navigationInput.y != 0)
            {
                if (navigationInput.y > .5f)
                    MoveCursor(-1);
                else if (navigationInput.y < .5f)
                    MoveCursor(1);
            }
            
            if (navigationInput.x != 0)
            {
                SliderTab sliderTab = tabs[current].GetComponent<SliderTab>();
                if (sliderTab)
                    sliderTab.ChangeValue (navigationInput.x);
            }
        };
        playerInputActions.UI.Navigation.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

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

        if (startOnFocus)
            OnFocus = true;
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
        playerInputActions.UI.Confirm.Disable();
    }
}
