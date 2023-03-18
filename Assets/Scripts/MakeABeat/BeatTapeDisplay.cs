using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTapeDisplay : MonoBehaviour
{
    public enum State { Off, Preview, On };

    public Gradient previewGradient;
    public float gradientScrollDuration;
    public GameObject fitNode;
    
    [Header("Preview Movement")]
    public Transform target;
    public float moveTowardsDuration;
    
    float color_t;
    float move_t;

    Vector3 startingPosition;

    State state;
    SpriteRenderer spriteRenderer;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPosition = transform.position;
    }

    void Start()
    {
        SetState (State.Off);    
    }

    private void Update() 
    {
        ColorizeRenderer();
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (state != State.Preview)
            return;

        if (target == null)
            return;

        move_t += Time.deltaTime;
        if (move_t > moveTowardsDuration * .5f)
            move_t = 0;

        transform.position = Vector3.Lerp(startingPosition, target.position, move_t / moveTowardsDuration);
    }

    private void ColorizeRenderer()
    {
        if (state != State.Preview)
            return;

        color_t += Time.deltaTime;
        if (color_t > gradientScrollDuration)
            color_t = 0;

        spriteRenderer.color = previewGradient.Evaluate( color_t / gradientScrollDuration );
    }

    public void SetSprite (Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetState(State state)
    {
        switch (state)
        {
            case State.Off:
                if (fitNode) 
                    fitNode.SetActive(false);
                spriteRenderer.enabled = false;
                transform.position = startingPosition;
                break;

            case State.Preview:
                if (fitNode)
                    fitNode.SetActive(false);
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.clear;
                transform.position = startingPosition;
                move_t = 0;
                color_t = 0;
                break;

            case State.On:
                if (fitNode)
                    fitNode.SetActive(true);
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.white;
                transform.position = startingPosition;
                break;
        }

        this.state = state;
    }
}
