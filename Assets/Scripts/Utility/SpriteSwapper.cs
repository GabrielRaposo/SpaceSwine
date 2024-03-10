using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [SerializeField] bool dontSetOnStart;
    [SerializeField] List<Sprite> sprites;

    SpriteRenderer spriteRenderer;    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!dontSetOnStart) 
            SetSpriteState(0);
    }

    public void SetSpriteState (int state)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sprites == null || sprites.Count < 1 || !spriteRenderer)
            return;

        //Debug.Log("Set state: " + state);
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
