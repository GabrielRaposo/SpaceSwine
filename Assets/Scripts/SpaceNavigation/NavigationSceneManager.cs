using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationSceneManager : MonoBehaviour
{
    private NavigationConsole _navigationConsole;

    public static NavigationSceneManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy (gameObject);
            return;
        }

        Instance = this;
    }

    public void ConectToConsole(NavigationConsole nc)
    {
        _navigationConsole = nc;
    }
    public void CloseAndSetScene(int sceneIndex)
    {
        SetShipTeleportScene(sceneIndex);
        _navigationConsole.ToggleConsoleState();
    }

    //TODO: mudar pra fazer por save
    private void SetShipTeleportScene(int sceneIndex)
    {
        NavigationConsole.ShipTeleportSceneIndex = sceneIndex;
        Debug.Log($"Setting scene: {NavigationConsole.ShipTeleportSceneIndex}");
    }
    
}
