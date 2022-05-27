using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorSwapper : MonoBehaviour
{
    SpriteRenderer sr;
    Color originalColor;

    private void Awake() 
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void SetColor(Color c)
    {
        if (sr)
            sr.color = c;
    }

    public void RestoreColor()
    {
        if (sr)
            sr.color = originalColor;
    }
}
