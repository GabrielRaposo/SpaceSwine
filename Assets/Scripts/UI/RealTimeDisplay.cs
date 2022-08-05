using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RealTimeDisplay : MonoBehaviour
{
    TextMeshProUGUI display;

    void Start()
    {
        display = GetComponentInChildren<TextMeshProUGUI>();
        if (!display)
        {
            enabled = false;
            return;
        }

        SetDisplay();
    }

    void Update()
    {
        SetDisplay();
    }

    private void SetDisplay()
    {
        string sufix = "AM";
        int hour = System.DateTime.Now.Hour;
        if (hour > 11)
            sufix = "PM";

        display.text = (hour % 12).ToString("0") + " " + sufix;
    }
}
