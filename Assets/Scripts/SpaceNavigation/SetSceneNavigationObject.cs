using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSceneNavigationObject : NavigationObject
{
    public BuildIndex scene;
    
    private void OnEnable()
    {
        interactAction += () =>
        {
            if (NavigationSceneManager.Instance == null)
            {
                Debug.Log("NAV SCENE MANAGER NOT FOUND");
                return;
            }
            NavigationSceneManager.Instance.CloseAndSetScene((int)scene);
        };
    }
}
