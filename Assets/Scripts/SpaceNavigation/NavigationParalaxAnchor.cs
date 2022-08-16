using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationParalaxAnchor : MonoBehaviour
{
    public static NavigationParalaxAnchor Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        if (Instance != this)
            Destroy(this.gameObject);
    }
}
