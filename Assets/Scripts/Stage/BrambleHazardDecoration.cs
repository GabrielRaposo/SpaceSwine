using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteRenderer))]
public class BrambleHazardDecoration : MonoBehaviour
{
    [SerializeField] float maxOffset;
    [SerializeField] List<Sprite> sprites;

    int index;
    Vector3 positionOffset;
    
    Spline spline;
    SpriteRenderer sr;

    private void Awake() 
    {
        sr = GetComponent<SpriteRenderer>(); 
    }

    public void Setup (Spline spline, int index)
    {
        this.spline = spline;
        this.index  = index;

        if (index < 0)
            index = 0;

        float offset = Random.Range(0f, maxOffset);
        positionOffset = RaposUtil.RotateVector (Vector2.up, Random.Range(0f, 360f)) * offset;
        UpdatePosition();

        RandomizeSprite();
    }

    private void RandomizeSprite()
    {
        if (sprites == null || sprites.Count < 1)
            return;

        sr.sprite = sprites[ Random.Range(0, sprites.Count) ];
        transform.localEulerAngles = Vector3.forward * 90 * Random.Range(0, 4);

        enabled = sr.sprite != null;
    }

    private void Update() 
    {
        if (spline == null)
            return;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.localPosition = spline.GetPosition(index % spline.GetPointCount()) + positionOffset;
    }
}
