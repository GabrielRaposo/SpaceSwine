using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Minigame;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] InputAction resetInputAction;
    [SerializeField] InputAction saveInputAction;
    [SerializeField] InputAction resetSaveInputAction;

    PauseSystem pauseSystem;
    PlayerInputActions playerInputActions;
    GGSConsole ggsConsole;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Start.performed += (ctx) => 
        {
            if (GGSConsole.TurnedOn)
                return;

            pauseSystem?.TogglePauseState();
        };
        playerInputActions.UI.Start.Enable();

        //playerInputActions.UI.Other.performed += (ctx) => 
        //{
        //    if (ggsConsole)
        //        ggsConsole.ToggleConsoleState();
        //};
        //playerInputActions.UI.Other.Enable();
        
        resetInputAction.Enable();
        saveInputAction.Enable();
        resetSaveInputAction.Enable();
    }

    void Start()
    {
        if (!SaveManager.Initiated)
        {
            SaveManager.Load();
        }

        #if UNITY_EDITOR
        CustomEditorInicialization.Initialize();
        #endif

        pauseSystem = PauseSystem.Instance; 
        ggsConsole = GGSConsole.Instance;

        //resetInputAction.performed += (ctx) => ResetScene();
        saveInputAction.performed += (ctx) => SaveManager.SaveAllData();
        resetSaveInputAction.performed += (ctx) => SaveManager.ResetSave();

        SetupPlayer(); // Deve ocorrer no Start()
    }

    private void SetupPlayer()
    {
        if (!playerObject)
            return;

        LocalGameplayState playerState = playerObject.GetComponent<LocalGameplayState>();
        if (!playerState || playerState.state != GameplayState.Exploration)
            return;

        SpawnManager spawnManager = SpawnManager.Instance;
        if (!spawnManager)
            return;

        Vector3 spawnPosition = spawnManager.GetSpawnPoint();
        playerObject.transform.position = spawnPosition;
        Debug.Log("set spawnPosition: " + spawnPosition);
    }

    public static void GoToScene (BuildIndex buildIndex)
    {
        SceneTransition.LoadScene((int) buildIndex);
    }

    public static void ResetScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneTransition.ReloadScene();
    }

    public static void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    private void OnDisable() 
    {
        playerInputActions.UI.Start.Disable();
        //playerInputActions.UI.Other.Disable();
        resetInputAction.Disable();    
        saveInputAction.Disable();
        resetSaveInputAction.Disable();
    }
}
