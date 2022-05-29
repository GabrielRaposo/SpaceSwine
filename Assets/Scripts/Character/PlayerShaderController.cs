using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShaderController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    private Material mat;

    private void Awake()
    {
        if(sprite == null) return;

        mat = sprite.material;
    }

    public void SetWhiteFade(float value)
    {
        if(mat == null) return;
        mat.SetFloat("_WhiteFade",value);
    }
    
}
