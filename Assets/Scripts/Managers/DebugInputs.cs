using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugInputs : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] InputAction displayListAction;
    [SerializeField] InputAction getAllAction;

    [Header("Danger Zone")]
    [SerializeField] InputAction previousRoundAction;
    [SerializeField] InputAction nextRoundAction;
    [SerializeField] InputAction clearStageAction;

    [Header("Save")]
    [SerializeField] InputAction unlockShipAccessAction;
    [SerializeField] InputAction saveInputAction;
    [SerializeField] InputAction resetSaveInputAction;

    [Header("Music")]
    [SerializeField] InputAction playMusicTestInput;
    [SerializeField] InputAction stopMusicTestInput;
    
    [Header("FPS")]
    [SerializeField] InputAction fpsInputAction;
    [SerializeField] FPSCounterDisplay fpsDisplay;

    static DebugInputs Instance;
    static bool wasSetup;

    private void Awake() 
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    private void OnEnable() 
    {
        if (Instance != this)
            return;
        
        displayListAction.Enable();
        getAllAction.Enable();

        previousRoundAction.Enable();
        nextRoundAction.Enable();
        clearStageAction.Enable();

        unlockShipAccessAction.Enable();
        saveInputAction.Enable();
        resetSaveInputAction.Enable();

        playMusicTestInput.Enable();
        stopMusicTestInput.Enable();
        
        fpsInputAction.Enable();

        if (wasSetup)
            return;


        /* -- Display -- */ {
            displayListAction.performed += (ctx) => 
            {
                StoryEventsManager.TogglePrintEventStates();
                DebugDisplay.Log("Toggle Print Events");
            };

            getAllAction.performed += (ctx) =>
            {
                StoryEventsManager.CompleteAll();
                DebugDisplay.Log ("All story events completed.");
            };
        }

        /* -- Danger Zone -- */ {
            previousRoundAction.performed += ctx => 
            {
                RoundsManager roundsManager = RoundsManager.Instance;
                if (!roundsManager)
                {
                    DebugDisplay.Log ("Input exclusive to Danger Zones.");
                    return;
                }

                roundsManager.PreviousRoundInput();
            };

            nextRoundAction.performed += ctx => 
            {
                RoundsManager roundsManager = RoundsManager.Instance;
                if (!roundsManager)
                {
                    DebugDisplay.Log ("Input exclusive to Danger Zones.");
                    return;
                }

                roundsManager.NextRoundInput();
            };

            clearStageAction.performed += ctx => 
            {
                RoundsManager roundsManager = RoundsManager.Instance;
                if (!roundsManager)
                {
                    DebugDisplay.Log ("Input exclusive to Danger Zones.");
                    return;
                }

                roundsManager.ClearStageInput();
            };
        }

        /* -- Save -- */ { 
            unlockShipAccessAction.performed += (ctx) =>
            {
                StoryEventsManager.UnlockShipAccess();
                DebugDisplay.Log ("Ship Access Unlocked.");
            };

            saveInputAction.performed += (ctx) => 
            { 
                SaveManager.Save();
                DebugDisplay.Log ("Manual Save.");
            };

            resetSaveInputAction.performed += (ctx) => 
            {
                SaveManager.ResetSave();
                DebugDisplay.Log ("Manual Save Reset.");
            };
        }
        
        /* -- Music -- */ {
            playMusicTestInput.performed += (ctx) => 
            {
                SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
                if (soundtrackManager)
                    soundtrackManager.PlayInput();
                DebugDisplay.Log ("Play/Skip track.");
            };

            stopMusicTestInput.performed += (ctx) =>
            {
                SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
                if (soundtrackManager)
                    soundtrackManager.StopInput();
                DebugDisplay.Log ("Stop track.");
            };
        }

        /* -- FPS -- */ {
            fpsInputAction.performed += (ctx) => 
            {
                if (fpsDisplay)
                    fpsDisplay.ToggleVisivility();
            };
        }

        wasSetup = true;
    }

    private void OnDisable() 
    {
        if (Instance != this)
            return;

        displayListAction.Disable();
        getAllAction.Disable();

        //#if UNITY_EDITOR
        previousRoundAction.Disable();
        nextRoundAction.Disable();
        clearStageAction.Disable();
        //#endif

        unlockShipAccessAction.Disable();
        saveInputAction.Disable();
        resetSaveInputAction.Disable();
        
        playMusicTestInput.Disable();
        stopMusicTestInput.Disable();

        fpsInputAction.Disable();    
    }
}
