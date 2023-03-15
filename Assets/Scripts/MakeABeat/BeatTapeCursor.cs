using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTapeCursor : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] Vector2 sizeExtension;
    [SerializeField] Gradient gradient;
    [SerializeField] List <SpriteRenderer> arrows;

    float t;
    bool highlighted;

    Vector2 startSize;

    SpriteRenderer display;

    private void Awake() 
    {
        display = GetComponent<SpriteRenderer>();
        startSize = display.size;
    }

    public void SetState (bool value)
    {
        //Debug.Log($"SetState ({value}) - {name}");
        display.enabled = value;
        if (arrows.Count > 0)
        {
            foreach (SpriteRenderer arrow in arrows)
            {
                arrow.enabled = value;
                if (!value)
                    arrow.color = gradient.Evaluate(0);
            }
        }

        if (!value)
        {
            display.size = startSize;
            display.color = gradient.Evaluate(0);
        }

        highlighted = value;
    }

    void Update()
    {
        if (!highlighted)
            return;

        t += Time.deltaTime;
        display.size = Vector2.Lerp(startSize, startSize + sizeExtension, t / duration);
        
        Color color = gradient.Evaluate(t / duration);
        display.color = color;
        if (arrows.Count > 0)
        {
            foreach (SpriteRenderer arrow in arrows)
                arrow.color = color;
        }

        if (t > duration)
            t = 0;
    }
}
