using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTapeDisplay : MonoBehaviour
{
    public enum State { Off, Preview, On };

    public Gradient previewGradient;
    public float gradientScrollDuration;
    public GameObject fitNode;
    
    float t;

    State state;
    SpriteRenderer spriteRenderer;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetState (State.Off);    
    }

    private void Update() 
    {
        if (state != State.Preview)
            return;

        t += Time.deltaTime;
        if (t > gradientScrollDuration)
            t = 0;

        spriteRenderer.color = previewGradient.Evaluate( t / gradientScrollDuration );
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
                break;

            case State.Preview:
                if (fitNode)
                    fitNode.SetActive(false);
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.clear;
                t = 0;
                break;

            case State.On:
                if (fitNode)
                    fitNode.SetActive(true);
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.white;
                break;
        }

        this.state = state;
    }
}
