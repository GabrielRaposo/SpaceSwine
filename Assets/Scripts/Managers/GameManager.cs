using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerObject;

    void Start()
    {
        if (playerObject)
        {
            Health health = playerObject.GetComponent<Health>();    
            if (health)
                health.OnDeathEvent += ResetScene;
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //    ResetScene();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
