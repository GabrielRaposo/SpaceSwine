using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeBoxItem : MonoBehaviour
{
    [SerializeField] float hiddenY;
    [SerializeField] float shownY;
    [SerializeField] float duration;

    float t;
    bool highlighted;

    BeatTapeScriptableObject beatTapeData;

    public BeatTapeScriptableObject BeatTape
    {
        get { return beatTapeData; }
    }

    void Start()
    {
        t = duration;
    }

    public void Setup (BeatTapeScriptableObject beatTapeData)
    {
        this.beatTapeData = beatTapeData;

        SpriteRenderer display = GetComponentInChildren<SpriteRenderer>();
        if (display)
            display.sprite = beatTapeData.sidewaysSprite;

        transform.localPosition = new Vector2(transform.localPosition.x, hiddenY); 
    }

    public void SetHighlighted(bool value, bool dontOverride = false) 
    {
        if (dontOverride && highlighted && !value)
            return;

        if (highlighted != value)
            t = 0;
        highlighted = value;
    }

    private void Update() 
    {
        t += Time.deltaTime;

        if (t > duration)
            t = duration;
        
        float y = Mathf.Lerp
        (
            highlighted ? hiddenY : shownY,
            highlighted ? shownY  : hiddenY,
            t / duration
        );

        transform.localPosition = new Vector2 (transform.localPosition.x, y);
    }
}
