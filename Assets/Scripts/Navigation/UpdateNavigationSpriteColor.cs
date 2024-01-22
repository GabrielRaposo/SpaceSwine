using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UpdateNavigationSpriteColor : MonoBehaviour
{
    public enum ColorType { Background, Selected, Unselected, Unavailable }

    [SerializeField] ColorType colorType;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        if (!Application.isEditor)
            return;

        if (spriteRenderer == null)
            return;

        NavigationWorldGroup worldGroup = GetComponentInParent<NavigationWorldGroup>();
        if (worldGroup == null) 
            return;

        switch (colorType)
        {
            case ColorType.Background:
                spriteRenderer.color = worldGroup.BackgroundColor;
                return;

            case ColorType.Selected:
                spriteRenderer.color = worldGroup.SelectedColor;
                return;

            case ColorType.Unselected:
                spriteRenderer.color = worldGroup.UnselectedColor;
                return;

            case ColorType.Unavailable:
                spriteRenderer.color = worldGroup.UnavailableColor;
                return;
        }
    }
}
