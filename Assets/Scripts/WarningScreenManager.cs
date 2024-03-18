using DevLocker.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningScreenManager : MonoBehaviour
{
    [SerializeField] float waitTime;
    [SerializeField] SceneReference nextScene;

    bool inputLocked;

    PlayerInputActions actionInputs;

    private void OnEnable()
    {
        inputLocked = true;

        actionInputs = new PlayerInputActions();
        actionInputs.Enable();

        actionInputs.UI.Confirm.performed += (ctx) => 
        {
            if (inputLocked)
                return;

            SceneTransition.LoadScene( nextScene.ScenePath, SceneTransition.TransitionType.BlackFade );
            enabled = false;
        };
        actionInputs.UI.Confirm.Enable();
    }

    void Start()
    {
        this.WaitSeconds(waitTime, UnlockInputs);
    }

    private void UnlockInputs()
    {
        inputLocked = false;
    }

    private void OnDisable()
    {
        actionInputs.Disable();
        actionInputs.UI.Confirm.Disable();
    }
}
