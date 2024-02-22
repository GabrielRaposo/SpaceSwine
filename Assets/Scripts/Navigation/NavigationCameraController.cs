using System;
using System.Collections;
using System.Collections.Generic;
using Minigame;
using UnityEngine;

public class NavigationCameraController : MonoBehaviour
{
    const float SCREEN_OFFSET = 3.25f;

    public NavigationShip ship;

    //[SerializeField] float lerpRatio;

    public static NavigationCameraController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetRenderTexture (RenderTexture renderTexture)
    {
        Camera camera = GetComponent<Camera>();
        if (!camera)
        {
            Debug.LogError("NAV CAMERA NOT FOUND");
            return;
        }
            

        camera.targetTexture = renderTexture;
    }
}
