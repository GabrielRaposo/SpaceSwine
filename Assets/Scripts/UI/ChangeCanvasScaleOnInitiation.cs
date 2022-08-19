using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ChangeCanvasScaleOnInitiation : MonoBehaviour
{
    CanvasScaler canvasScaler;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        if (!canvasScaler)
            return;

        float defaultRatio = 16f / 9f;
        float currentRatio = Screen.width / Screen.height;

        if (currentRatio > defaultRatio)
        {
            canvasScaler.matchWidthOrHeight = 1;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
    }
}