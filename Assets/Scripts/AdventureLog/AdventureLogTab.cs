using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdventureLogTab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;

    public void Setup (string text)
    {
        if (textDisplay)
            textDisplay.text = text;

        SetActiveState(true);
    }

    public void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }

}
