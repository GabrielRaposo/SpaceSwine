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

    RoundsManager roundsManager;
    PauseSystem pauseSystem;
    PlayerInputActions playerInputActions;
    GGSConsole ggsConsole;

    public static bool BlockCharacterInput;
    public static bool OnDialogue;
    public static PlaylistScriptableObject CurrentPlaylist; 

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Start.performed += (ctx) => 
        {
            if (BlockCharacterInput || OnTransition || OnDialogue)
                return;

            pauseSystem?.TogglePauseState();
        };
        playerInputActions.UI.Start.Enable();

        playerInputActions.UI.Reset.performed += (ctx) => 
        {
            if (BlockCharacterInput || OnTransition || PauseSystem.OnPause)
                return;

            if (RoundsManager.Instance)
                RoundsManager.Instance.CallReset();
        };
        playerInputActions.UI.Reset.Enable();
        
        #if UNITY_EDITOR
            resetInputAction.Enable();
            saveInputAction.Enable();
            resetSaveInputAction.Enable();
        #endif
    }

    void Start()
    {
        pauseSystem = PauseSystem.Instance; 
        ggsConsole = GGSConsole.Instance;

        #if UNITY_EDITOR
            //resetInputAction.performed += (ctx) => ResetScene();
            saveInputAction.performed += (ctx) => SaveManager.SaveAllData();
            resetSaveInputAction.performed += (ctx) => SaveManager.ResetSave();
        #endif

        SetupPlayer(); // Deve ocorrer no Start()
        SetupPlaylist();
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
    }

    private void SetupPlaylist()
    {
        if (CurrentPlaylist == null)
            return;

        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (!soundtrackManager)
            return;

        soundtrackManager.SetPlaylist(CurrentPlaylist);
    }

    public static void GoToScene (string path)
    {
        SceneTransition.LoadScene(path, SceneTransition.TransitionType.BlackFade);
    }

    public static void GoToScene (BuildIndex index)
    {   
        string path = string.Empty;
        switch (index)
        {
            default:
            case BuildIndex.Splash:
                path = "Assets/Scenes/SplashScene.unity";
                break;

            case BuildIndex.Title:
                path = "Assets/Scenes/TitleScene.unity";
                break;

            case BuildIndex.Ship:
                path = "Assets/Scenes/ShipScene.unity";
                break;

            case BuildIndex.NavigationScene:
                path = "Assets/Scenes/NavigationScene.unity";
                break;

            case BuildIndex.MakeABeat:
                path = "Assets/Scenes/MakeABeatScene.unity";
                break;
        }

        SceneTransition.LoadScene(path, SceneTransition.TransitionType.BlackFade);
    }

    public static void ResetScene()
    {
        SceneTransition.ReloadScene();
    }

    public static void QuitGame()
    {
        #if UNITY_EDITOR
        Debug.Log("Quit game");
        #endif

        Application.Quit();
    }

    public static bool IsOnScene (BuildIndex buildIndex)
    {
        return SceneManager.GetActiveScene().buildIndex == (int) buildIndex;
    }

    public static bool OnTransition
    {
        get { return SceneTransition.OnTransition || RoundTransition.OnTransition; }
    }

    public static string CurrentScene
    {
        get { return SceneManager.GetActiveScene().path; }
    }

    private void OnDisable() 
    {
        playerInputActions.UI.Start.Disable();
        playerInputActions.UI.Reset.Disable();

        #if UNITY_EDITOR
            resetInputAction.Disable();    
            saveInputAction.Disable();
            resetSaveInputAction.Disable();
        #endif
    }
}
