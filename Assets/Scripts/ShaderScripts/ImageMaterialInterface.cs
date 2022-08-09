using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ImageMaterialInterface : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] Material mat;

    public Animator animator;
    public float x;
    public float armRange;

    public UnityAction onAnimationEnd;
    
    private void Update()
    {
        if (mat == null)
        {
            if (img != null)
            {
                mat = img.material;
            }
            return;
        }

        if (img != null && mat != img.material)
            mat = img.material;
        
        mat.SetFloat("_x", x);
        mat.SetFloat("_armRange", armRange);
    }

    public void OnAnimationEnd()
    {
        onAnimationEnd?.Invoke();
    }
}
