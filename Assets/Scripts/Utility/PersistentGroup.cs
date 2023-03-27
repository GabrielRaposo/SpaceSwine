using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentGroup : MonoBehaviour
{
    static PersistentGroup Instance;

    private void Awake() 
    {
        if (Instance)
        {
            Destroy (gameObject);
            return;
        }

        Application.targetFrameRate = 60;

        DontDestroyOnLoad (gameObject);
        Instance = this;
    }
}
