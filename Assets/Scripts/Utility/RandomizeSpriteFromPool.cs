using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomizeSpriteFromPool : MonoBehaviour
{
    [SerializeField] public List<Sprite> sprites;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] private bool hasBeenSetup;    

    void OnValidate()
    {
        return;

        if (hasBeenSetup)
            return;

        if (sprites == null || sprites.Count < 1)
            return;

        if (!spriteRenderer)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (!spriteRenderer)
            return;

        Setup();
    }

    protected void Setup()
    {
        int index = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[index % sprites.Count];

        hasBeenSetup = true;
    }

    public void RandomizeSprite()
    {
        int index = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[index % sprites.Count];
    }

    public void ForceSpecificSprite(int index)
    {
        spriteRenderer.sprite = sprites[index];
    }
    
}
