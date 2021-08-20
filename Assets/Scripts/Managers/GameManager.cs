using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] InputAction resetInputAction;

    private void OnEnable() 
    {
        resetInputAction.Enable();
    }

    void Start()
    {
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
        resetInputAction.Disable();    
    }
}
