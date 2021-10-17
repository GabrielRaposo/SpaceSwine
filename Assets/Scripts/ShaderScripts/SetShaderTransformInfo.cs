using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetShaderTransformInfo : MonoBehaviour
{
    private Material mat;

    private void Update()
    {
        if(mat == null)  mat = GetComponent<SpriteRenderer>().sharedMaterial;
        if(mat == null) return;
        
        mat.SetVector("_Pos", transform.position);
        mat.SetFloat("_Scale", transform.localScale.x);
        mat.SetFloat("_Rotation", transform.eulerAngles.z);
        
    }
}
