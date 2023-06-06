using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamWorksSingleton : MonoBehaviour
{
    static SteamWorksSingleton Instance;

    private void Awake() 
    {
        if (Instance)
        {
            Destroy (gameObject);
            return;
        }

        DontDestroyOnLoad (gameObject);
        Instance = this;
    }
}
