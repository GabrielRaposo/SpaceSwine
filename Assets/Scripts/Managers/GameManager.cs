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

        if (playerObject)
        {
            Health health = playerObject.GetComponent<Health>();    
            if (health)
                health.OnDeathEvent += ResetScene;
        }

        resetInputAction.performed += (ctx) => ResetScene();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable() 
    {
        playerInputActions.UI.Start.Disable();
        resetInputAction.Disable();    
    }
}
