using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanguageScreenFunctions : MonoBehaviour
{
    [SerializeField] List<LanguageMenuButton> languageButtons;
    [SerializeField] float holdCooldown;

    [Header("Sequence")]
    [SerializeField] CanvasGroup canvasGroup;

    [HideInInspector] public bool OnFocus;

    int current = -1;
    float holdCount;

    PlayerInputActions playerInputActions;
    InputAction axisInput;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        axisInput = playerInputActions.UI.Navigation;
        axisInput.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            SetLanguage ( languageButtons[current].LocalizationCode );
        };
        playerInputActions.UI.Confirm.Enable();

        if (canvasGroup)
            canvasGroup.alpha = 1;
    }

    private void Start() 
    {
        current = ( LocalizationManager.CurrentLanguage == GameLocalizationCode.EN ? 1 : 0 );
        SelectCurrent();
    }

    private void Update() 
    {
        Vector2 axis = axisInput.ReadValue<Vector2>();
        if (axis == Vector2.zero)
            holdCount = 0;
        else
            holdCount -= Time.deltaTime;

        if (holdCount < 0)
            holdCount = 0;

        if (!OnFocus || SceneTransition.OnTransition)
            return;
        
        if (holdCount > 0)
            return;

        if (axis.y != 0)
        {
            if (axis.y > .75f)
                MoveCursor(-1);
            else if (axis.y < -.75f)
                MoveCursor(1);
        }
    }

    private void MoveCursor (int direction)
    {
        holdCount = holdCooldown;

        current += direction;
        if (current < 0)
            current = languageButtons.Count - 1;
        current %= languageButtons.Count;

        SelectCurrent();
    }

    private void SelectCurrent()
    {
        for (int i = 0; i < languageButtons.Count; i++)
            languageButtons[i].Deselect();

        languageButtons[current].Select();
    }

    private void OnDisable() 
    {
        axisInput.Disable();
        playerInputActions.UI.Confirm.Disable();

        if (canvasGroup)
            canvasGroup.alpha = 0;
    }

    private void SetLanguage (GameLocalizationCode code)
    {
        LocalizationManager.CurrentLanguage = code;

        TitleStateManager titleStateManager = GetComponentInParent<TitleStateManager>();
        if (!titleStateManager)
            return;

        titleStateManager.SetIntroState();
    }
}
