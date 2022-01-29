using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetMaterialCreator : MonoBehaviour
{
    public Material baseMaterial;
    public Material currentMaterial;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] public GravitationalPlanet planet;

    private void OnValidate()
    {
        SetPixelSize();
        
        if(gameObject.scene.name == "") return;
        if(gameObject.scene.name == gameObject.name) return;
    
        if (currentMaterial == null)
        {
            if(baseMaterial == null) return;
            
            currentMaterial = new Material(baseMaterial);
            currentMaterial.name = currentMaterial.name + " " + Random.Range(0, 9999).ToString("X");
            TweakMaterial(currentMaterial);
            
            if(_spriteRenderer == null) return;

            SetPixelSize();
            
            _spriteRenderer.material = currentMaterial;
        }
            
    }

    private void SetPixelSize()
    {
        Debug.Log($"SetPixelSize()");

        if (planet == null)
        {
            Debug.Log("NULL PLANET");
            return;
        }

        if (currentMaterial == null)
        {
            Debug.Log("NULL MATERIAL");
            return;
        }
        Debug.Log($"PixelSize: {115f*planet.GetAttributes().planetRadius}");
        currentMaterial.SetFloat("_PixelSize", 115f*planet.GetAttributes().planetRadius);
    }

    public void ClarMaterial()
    {
        currentMaterial = null;
        OnValidate();
    }
    
    private void TweakMaterial(Material mat)
    {
        //TweakVectorValue(mat, "_SphereOffset", v => new Vector3(Random.Range(0f,999f),Random.Range(0f,999f),Random.Range(0f,999f)));
        
        TweakFloatValue(mat, "_SpotsCount1", f => f * Random.Range(0.8f,1.2f));
        TweakFloatValue(mat, "_SpotsCount2", f => f * Random.Range(0.8f,1.2f));
        TweakFloatValue(mat, "_SpotsCount3", f => f * Random.Range(0.8f,1.2f));
        //
        TweakFloatValue(mat, "_SpotsSize1", f => f * Random.Range(0.8f,1.2f));
        TweakFloatValue(mat, "_SpotsSize2", f => f * Random.Range(0.8f,1.2f));
        TweakFloatValue(mat, "_SpotsSize3", f => f * Random.Range(0.8f,1.2f));
        //
        TweakFloatValue(mat, "_NoiseRotation1", f => Random.Range(0f,360f));
        TweakFloatValue(mat, "_NoiseRotation2", f => Random.Range(0f,360f));
        TweakFloatValue(mat, "_NoiseRotation3", f => Random.Range(0f,360f));
        //
        TweakVectorValue(mat, "_SpotsShape1", v => v*Random.Range(0.75f,1.25f));
        TweakVectorValue(mat, "_SpotsShape2", v => v*Random.Range(0.75f,1.25f));
        TweakVectorValue(mat, "_SpotsShape3", v => v*Random.Range(0.75f,1.25f));
        //
        TweakVectorValue(mat, "_NoiseOffset1", v => new Vector3(Random.Range(0f,999f),Random.Range(0f,160f),Random.Range(0f,160f)));
        TweakVectorValue(mat, "_NoiseOffset2", v => new Vector3(Random.Range(0f,999f),Random.Range(0f,160f),Random.Range(0f,160f)));
        TweakVectorValue(mat, "_NoiseOffset3", v => new Vector3(Random.Range(0f,999f),Random.Range(0f,160f),Random.Range(0f,160f)));
        
    }

    private void TweakFloatValue(Material mat, string valueName, Func<float,float> action)
    {
        mat.SetFloat(valueName, action(mat.GetFloat(valueName)));
    }

    private void TweakVectorValue(Material mat, string valueName, Func<Vector4, Vector4> action)
    {
        mat.SetVector(valueName, action(mat.GetVector(valueName)));
    }
    
}
