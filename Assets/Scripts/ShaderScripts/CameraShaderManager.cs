using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraShaderManager : MonoBehaviour
{
    
    public BlitRenderFeature dangerAreaShader;
    public BlitRenderFeature distortionShader;
    [SerializeField] private ForwardRendererData _rendererData;

    private void Awake()
    {
    }

    private void Update()
    {
            
    }

    public void SetDangerAreaShader(bool value)
    {
        if (value)
        {
            _rendererData.rendererFeatures.Add(distortionShader);
        }
        else
        {
            _rendererData.rendererFeatures.Clear();
        }
        
    }
    
}
