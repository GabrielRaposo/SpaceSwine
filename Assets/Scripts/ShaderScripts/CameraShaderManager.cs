using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class CameraShaderManager : MonoBehaviour
{
    private Camera cam;
    
    private ForwardRendererData baseRendererData;
    private ForwardRendererData dangerAreaRendererData;

    public static CameraShaderManager Instance;

    private UniversalAdditionalCameraData _cameraData; 
    
    private void Awake()
    {
        cam = Camera.main;
        _cameraData = cam.GetUniversalAdditionalCameraData();
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetDangerAreaShader(bool value)
    {
        Debug.Log($"SetDangerAreaShader {value}");
        if (value)
        {
            _cameraData.SetRenderer(1);
        }
        else
        {
            _cameraData.SetRenderer(0);
        }

    }

}
