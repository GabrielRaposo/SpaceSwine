﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTapeCursor : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] Vector2 sizeExtension;
    [SerializeField] Gradient gradient;
    [SerializeField] GameObject arrowPrefab;

    List <(Transform transf, SpriteRenderer renderer)> arrows;

    float t;
    bool highlighted;

    Vector2 startSize;

    SpriteRenderer display;

    private void Awake() 
    {
        display = GetComponent<SpriteRenderer>();

        startSize = display.size;
        arrows = new List<(Transform transf, SpriteRenderer renderer)>();
    }

    public void InitArrows (List <BeatNavigationItem.NavigationData> dataList)
    {
        if (!arrowPrefab || dataList.Count < 1)
            return;

        arrows = new List<(Transform transf, SpriteRenderer renderer)>();

        foreach (var data in dataList)
        {
            GameObject arrowObj = Instantiate(arrowPrefab, transform);
            {
                Vector2 origin = transform.position;
                float angle = Vector2.SignedAngle(Vector2.up, data.Direction(origin));
                arrowObj.transform.eulerAngles = Vector3.forward * (angle + 90);
                arrowObj.transform.position = transform.position + (data.Direction(origin) * .75f);
            }
            
            SpriteRenderer arrowRenderer = arrowObj.GetComponentInChildren<SpriteRenderer>();

            arrows.Add ( (arrowObj.transform, arrowRenderer) );
        }

        arrowPrefab.SetActive(false);
    }

    public void SetState (bool value)
    {
        //Debug.Log($"SetState ({value}) - {name}");
        display.enabled = value;
        if (arrows.Count > 0)
        {
            foreach (var arrow in arrows)
            {
                arrow.renderer.enabled = value;
                if (!value)
                    arrow.renderer.color = gradient.Evaluate(0);
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
            foreach (var arrow in arrows)
            {
                arrow.renderer.color = color;
                arrow.renderer.transform.localPosition = Vector2.Lerp(Vector2.right * .05f, Vector2.zero, t / duration);
            }
        }

        if (t > duration)
            t = 0;
    }
}
