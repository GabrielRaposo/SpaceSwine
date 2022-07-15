using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSceneNavigationObject : NavigationObject
{
    private NavigationSceneManager _navigationSceneManager;
    public BuildIndex scene;
    private void Awake()
    {
        _navigationSceneManager = NavigationSceneManager.Instance;

        interactAction += () =>
        {
            if (_navigationSceneManager == null)
            {
                Debug.Log("NAV SCENE MANAGER NOT FOUND");
                return;
            }
            _navigationSceneManager.CloseAndSetScene((int)scene);
        };
    }
}
