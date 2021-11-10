using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageSwapper : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;
    [SerializeField] List<Color> colors;

    Image image;    

    void Start()
    {
        image = GetComponent<Image>();
        SetSpriteState(0);
    }

    public void SetSpriteState (int state)
    {
        if (sprites == null || sprites.Count < 1 || !image)
            return;

        image.sprite = sprites[state % sprites.Count];

        if (colors == null)
            return;

        image.color = colors[state % colors.Count];
    }

    public void SetVisibility (bool value)
    {
        image.enabled = value;
    }
}
