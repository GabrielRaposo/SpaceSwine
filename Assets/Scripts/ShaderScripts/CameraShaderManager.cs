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
    private string m_extraDispDir = "_ExtraDisplacementDirection";
    private string m_extraDispVal = "_ExtraDisplacement";
    private string m_borderRatio = "_BorderRatio";
    
    private float initBorderRatio;
    private float initExtraDisp;
    
    private void ResetDangerSceneMaterial()
    {
        dangerSceneMat.SetFloat(m_sphereRadius, 0f);
        dangerSceneMat.SetFloat(m_borderRatio, initBorderRatio);
        dangerSceneMat.SetFloat(m_extraDispVal, initExtraDisp);
    }
    
    private void SetDangerSceneValues()
    {
        initBorderRatio = dangerSceneMat.GetFloat(m_borderRatio);
        initExtraDisp = dangerSceneMat.GetFloat(m_extraDispVal);
    }
    
    public void StartDeathSentence(Vector2 position, UnityAction OnCompleteAnimation)
    {
        //_ExtraDisplacementDirection rodando

        Sequence mainSequence = DOTween.Sequence();
        
        dangerSceneMat.SetVector(m_spherePos, position);
        
        //var sphereTGrow = dangerSceneMat.DOFloat(m_sphereRadius, 0.3f,0.7f).SetEase(Ease.OutBounce);
        //var sphereTShrink = mainSequence.Append(dangerSceneMat.DOFloat(m_sphereRadius, 0.0f, 0.2f));

        //Sequence sphereSequence = DOTween.Sequence();

        //sphereSequence.Append(sphereTGrow);
        //sphereSequence.Append(sphereTShrink);
        
        //mainSequence.Join(sphereSequence);

        mainSequence.Join(dangerSceneMat.DOFloat(m_extraDispVal, 2f, 0.05f));
        
        mainSequence.Join(dangerSceneMat.DOFloat(m_borderRatio, initBorderRatio * 8f, 0.05f));

        //mainSequence.App
        
        mainSequence.Append(dangerSceneMat.DOFloat(m_borderRatio, initBorderRatio, 0.18f));
        
        mainSequence.Join(dangerSceneMat.DOFloat(m_extraDispVal, initExtraDisp, 0.18f));

        mainSequence.OnComplete(() =>
        {
            StartCoroutine(ResetAfterScene(OnCompleteAnimation));
        });

    }

    private IEnumerator ResetAfterScene(UnityAction a)
    {
        yield return new WaitForSeconds(0.5f);
        a?.Invoke();
        ResetDangerSceneMaterial();
    }

}
