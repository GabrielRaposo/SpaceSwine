using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputIcon : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] string keyboard1InteractID;
    [SerializeField] string keyboard2InteractID;
    [SerializeField] string joystickInteractID;

    [Header("References")]
    [SerializeField] SpriteRenderer boxRenderer;
    [SerializeField] TextMeshPro textDisplay;

    void Start()
    {
        UpdateBoxRenderer();
        InputTagController.OnInputTypeChanged += UpdateBoxRenderer;
    }

    private void UpdateBoxRenderer()
    {
        if (!boxRenderer || !textDisplay)
            return;

        switch (InputTagController.CurrentControllerType)
        {
            case ControllerType.Keyboard:
                boxRenderer.enabled = true;
                textDisplay.text = LocalizationManager.GetInputText (keyboard1InteractID);
                break;

            case ControllerType.XboxJoystick:
                boxRenderer.enabled = false;
                textDisplay.text = LocalizationManager.GetInputText (joystickInteractID);
                break;
        }
    }
}
