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
        

        /* -- Display -- */ {
            displayListAction.performed += (ctx) => 
            {
                StoryEventsManager.TogglePrintEventStates();
            };
            displayListAction.Enable();

            getAllAction.performed += (ctx) =>
            {
                StoryEventsManager.CompleteAll();
                DebugDisplay.Log ("All story events completed.");
            };
            getAllAction.Enable();
        }

        /* -- Danger Zone -- */ {
//#if UNITY_EDITOR
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
            previousRoundAction.Enable();

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
            nextRoundAction.Enable();

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
            clearStageAction.Enable();
//#endif
        }

        /* -- Save -- */ { 
            unlockShipAccessAction.performed += (ctx) =>
            {
                StoryEventsManager.UnlockShipAccess();
                DebugDisplay.Log ("Ship Access Unlocked.");
            };
            unlockShipAccessAction.Enable();

            saveInputAction.performed += (ctx) => 
            { 
                SaveManager.Save();
                DebugDisplay.Log ("Manual Save.");
            };
            saveInputAction.Enable();

            resetSaveInputAction.performed += (ctx) => 
            {
                SaveManager.ResetSave();
                DebugDisplay.Log ("Manual Save Reset.");
            };
            resetSaveInputAction.Enable();
        }
        
        /* -- Music -- */ {
            playMusicTestInput.performed += (ctx) => 
            {
                SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
                if (soundtrackManager)
                    soundtrackManager.PlayInput();
                DebugDisplay.Log ("Play/Skip track.");
            };
            playMusicTestInput.Enable();

            stopMusicTestInput.performed += (ctx) =>
            {
                SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
                if (soundtrackManager)
                    soundtrackManager.StopInput();
                DebugDisplay.Log ("Stop track.");
            };
            stopMusicTestInput.Enable();
        }

        /* -- FPS -- */ {
            fpsInputAction.performed += (ctx) => 
            {
                if (fpsDisplay)
                    fpsDisplay.ToggleVisivility();
            };
            fpsInputAction.Enable();
        }
    }

    private void OnDisable() 
    {
        if (Instance != this)
            return;

        displayListAction.Disable();

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
