﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugDisplay : MonoBehaviour
{
    [SerializeField] float duration;

    [Header("References")]
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] CanvasGroup canvasGroup;

    public static DebugDisplay Instance;

    private void Awake() 
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    private void Start() 
    {
        SetState(false);
    }

    private void SetState (bool value)
    {
        if (!textDisplay || !canvasGroup)
            return;

        canvasGroup.alpha = value ? 1 : 0;
        textDisplay.text = string.Empty;
    }

    public static void Call (string s)
    {
        if (Instance == null)
            return;

        Instance.LocalCallText(s);
    }

    public void LocalCallText(string s)
    {
        if (!textDisplay || !canvasGroup)
            return;

        SetState(true);
        textDisplay.text = s;

        this.WaitSeconds(duration, () => SetState(false));
    }
}
