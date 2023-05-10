using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class FPSCounterDisplay : MonoBehaviour
{
    [SerializeField] bool showAtStart;
    [SerializeField] bool ceilValue;
    [SerializeField] TextMeshProUGUI display;


    bool showing;
    float deltaTime;

    void Start()
    {
        SetVisibility (showAtStart);
    }

    private void Update() 
    {
        if (!showing)
            return;

        if (display == null)
            return;

         deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
         float fps = 1.0f / deltaTime;
         display.text = "FPS ";
         display.text += ceilValue ? Mathf.Ceil(fps).ToString() : fps.ToString ("0.0");
    }

    private void SetVisibility (bool value)
    {
        if (display == null)
            return;

        display.enabled = value;
        showing = value;
    }

    public void ToggleVisivility()
    {
        SetVisibility (!showing);
    }
}
