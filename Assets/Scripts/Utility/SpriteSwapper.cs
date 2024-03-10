using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;

    SpriteRenderer spriteRenderer;    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpriteState(0);
    }

    public void SetSpriteState (int state)
    {
        if (sprites == null || sprites.Count < 1 || !spriteRenderer)
            return;

        spriteRenderer.sprite = sprites[state % sprites.Count];
    }

    public void SetVisibility (bool value)
    {
        spriteRenderer.enabled = value;
    }

    public void ChangePrintOrder (int value)
    {
        spriteRenderer.sortingOrder = value;
    }
}
