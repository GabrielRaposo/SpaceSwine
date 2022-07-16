using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChillMenuNavigation : MonoBehaviour
{
    [SerializeField] bool startOnFocus;

    [HideInInspector] public bool OnFocus;

    PlayerInputActions playerInputActions;
    TitleStateManager titleStateManager;

    private void Awake() 
    {
        titleStateManager = GetComponentInParent<TitleStateManager>();    
    }

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
        };
        playerInputActions.UI.Navigation.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            //titleButtons[current].Submit();
            if (titleStateManager)
                titleStateManager.SetMenuState();
        };
        playerInputActions.UI.Confirm.Enable();
    }

    private void Start() 
    {
        //current = 0;
        SelectCurrent();
        
        if (startOnFocus)
            OnFocus = true;
    }

    private void SelectCurrent()
    {
        //for (int i = 0; i < titleButtons.Count; i++)
        //{
        //    titleButtons[i].Deselect();
        //}
        //titleButtons[current].Select();
    }

    private void MoveCursor (int direction)
    {
        //current += direction;
        //if (current < 0)
        //    current = titleButtons.Count - 1;
        //current %= titleButtons.Count;

        //SelectCurrent();
    }

    private void OnDisable() 
    {
        playerInputActions.UI.Navigation.Disable();
        playerInputActions.UI.Confirm.Disable();
    }

}
