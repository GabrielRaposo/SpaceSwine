using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Hazard : MonoBehaviour
{
    const float LENGTH = 0.5f;

    [SerializeField] Vector2Int size;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        spriteRenderer.size = (Vector2) size * LENGTH;
        boxCollider.size = (Vector2) size * LENGTH; 
    }
}
