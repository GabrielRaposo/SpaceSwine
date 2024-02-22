using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNavigationDecorationsColor : MonoBehaviour
{
    private void OnValidate()
    {
        NavigationWorldGroup worldGroup = GetComponentInParent<NavigationWorldGroup>();
        if (worldGroup == null)
            return;

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers == null )
            return;

        foreach( SpriteRenderer spriteRenderer in spriteRenderers )
            spriteRenderer.color = worldGroup.UnselectedColor;
    }
}
