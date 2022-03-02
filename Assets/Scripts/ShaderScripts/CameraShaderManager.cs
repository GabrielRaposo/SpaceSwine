using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraShaderManager : MonoBehaviour
{
    [SerializeField] private ForwardRendererData _rendererData;

    private void Awake()
    {
        SetDangerAreaShader(true);
    }

    public void SetDangerAreaShader(bool value)
    {
        if (value)
        {
            var a = _rendererData.rendererFeatures;
        }
        
    }
    
}
