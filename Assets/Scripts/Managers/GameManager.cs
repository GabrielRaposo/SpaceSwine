using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] InputAction resetInputAction;

    PauseSystem pauseSystem;
    PlayerInputActions playerInputActions;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Start.performed += (ctx) => 
        {
            pauseSystem?.TogglePauseState();
        };
        playerInputActions.UI.Start.Enable();
        
        resetInputAction.Enable();
    }

    void Start()
    {
        pauseSystem = PauseSystem.Instance; 

        //if (playerObject)
        //{
        //    Health health = playerObject.GetComponent<Health>();    
        //    if (health)
        //        health.OnDeathEvent += ResetScene;
        //}

        resetInputAction.performed += (ctx) => ResetScene();

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
        resetInputAction.Disable();    
    }
}
