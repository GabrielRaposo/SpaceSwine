using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugInputs : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] InputAction displayListAction;
    [SerializeField] InputAction getAllAction;

    [Header("Save")]
    [SerializeField] InputAction unlockShipAcessAction;
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
                DebugDisplay.Call ("All story events completed.");
            };
            getAllAction.Enable();
        }

        /* -- Save -- */ { 
            unlockShipAcessAction.performed += (ctx) =>
            {
                StoryEventsManager.UnlockShipAccess();
                DebugDisplay.Call ("Ship Access Unlocked.");
            };
            unlockShipAcessAction.Enable();

            saveInputAction.performed += (ctx) => 
            { 
                SaveManager.Save();
                DebugDisplay.Call ("Manual Save.");
            };
            saveInputAction.Enable();

            resetSaveInputAction.performed += (ctx) => 
            {
                SaveManager.ResetSave();
                DebugDisplay.Call ("Manual Save Reset.");
            };
            resetSaveInputAction.Enable();
        }
        
        /* -- Music -- */ {
            playMusicTestInput.performed += (ctx) => 
            {
                SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
                if (soundtrackManager)
                    soundtrackManager.PlayInput();
                DebugDisplay.Call ("Play/Skip track.");
            };
            playMusicTestInput.Enable();

            stopMusicTestInput.performed += (ctx) =>
            {
                SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
                if (soundtrackManager)
                    soundtrackManager.StopInput();
                DebugDisplay.Call ("Stop track.");
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

        unlockShipAcessAction.Disable();
        saveInputAction.Disable();
        resetSaveInputAction.Disable();
        
        playMusicTestInput.Disable();
        stopMusicTestInput.Disable();

        fpsInputAction.Disable();    
    }
}
