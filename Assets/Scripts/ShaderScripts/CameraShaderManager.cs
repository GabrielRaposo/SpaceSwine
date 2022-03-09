using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class CameraShaderManager : MonoBehaviour
{
    private Camera cam;
    
    private ForwardRendererData baseRendererData;
    private ForwardRendererData dangerAreaRendererData;

    [SerializeField] private Material dangerSceneMat;

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

        SetDangerSceneValues();

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

    private string m_spherePos = "_SpherePosition";
    private string m_sphereRadius = "_SphereRadius";
    private string m_extraDisp = "_ExtraDisplacementDirection";
    private string m_borderRatio = "_BorderRatio";

    private float initBorderRatio;
    
    private void ResetDangerSceneMaterial()
    {
        dangerSceneMat.SetFloat(m_sphereRadius, 0f);
        dangerSceneMat.SetFloat(m_borderRatio, initBorderRatio);
    }
    
    private void SetDangerSceneValues()
    {
        initBorderRatio = dangerSceneMat.GetFloat(m_borderRatio);
    }
    
    public void StartDeathSentence(Vector2 position, UnityAction OnCompleteAnimation)
    {
        //_ExtraDisplacementDirection rodando

        Sequence s = DOTween.Sequence();
        
        dangerSceneMat.SetVector(m_spherePos, position);
        
        var sphereT = dangerSceneMat.DOFloat(m_sphereRadius, 0.3f,0.8f).SetEase(Ease.OutBounce);

        s.Join(sphereT);

        s.Join(dangerSceneMat.DOFloat(m_borderRatio, initBorderRatio * 10f, 0.4f)).SetEase(Ease.Flash);

        s.OnComplete(() =>
        {
            OnCompleteAnimation?.Invoke();
            ResetDangerSceneMaterial();
        });

    }

}
