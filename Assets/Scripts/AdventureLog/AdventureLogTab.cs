using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdventureLogTab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;

    public AdventureLogScriptableObject data { get; private set; }

    public void Setup (AdventureLogScriptableObject data)
    {
        this.data = data;

        if (textDisplay)
            textDisplay.text = data.fallbackText; // -- pegar do localization text

        SetActiveState(true);
    }

    public void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }

}
