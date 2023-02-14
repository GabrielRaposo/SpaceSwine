using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class HoldToSkipScene : MonoBehaviour
{
    [SerializeField] float holdDuration;
    [SerializeField] string baseText;
    [SerializeField] string localizationCode;
    [SerializeField] TextMeshProUGUI warningDisplay;
    [SerializeField] UnityEvent OnCallEvent;

    PlayerInputActions playerInputActions;
    InputAction startInput;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        startInput = playerInputActions.UI.Start;
        startInput.performed += (ctx) => 
        {
            SetHold();
        };
        startInput.canceled += (ctx) => 
        {
            CancelHold();
        };
        startInput.Enable();
    }

    private void Start() 
    {
        if (!warningDisplay)
        {
            enabled = false;
            return;
        }

        warningDisplay.text = string.Empty;
    }

    private void SetHold()
    {
        StopAllCoroutines();

        StartCoroutine( MainSequence() );
    }

    private IEnumerator MainSequence()
    {
        float stepDuration = holdDuration / 5;

        string localizedText = baseText;
        if (localizationCode != string.Empty) 
            localizedText = LocalizationManager.GetUiText(localizationCode, "Skipping scene");

        warningDisplay.text = localizedText;
        yield return new WaitForSeconds(stepDuration);

        warningDisplay.text += ".";
        yield return new WaitForSeconds(stepDuration);

        warningDisplay.text += ".";
        yield return new WaitForSeconds(stepDuration);

        warningDisplay.text += ".";
        yield return new WaitForSeconds(stepDuration);

        warningDisplay.text += "!";
        OnCallEvent?.Invoke();
    }

    private void CancelHold()
    {
        StopAllCoroutines();

        warningDisplay.text = string.Empty;
    }

    private void OnDisable() 
    {
        playerInputActions.Disable();

        startInput.Disable();
    }
}
