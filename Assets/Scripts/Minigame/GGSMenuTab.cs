using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GGSMenuTab : MonoBehaviour
{
    [SerializeField] Color highlightColor;
    [SerializeField] Color disabledColor;
    [SerializeField] AK.Wwise.Event submitAKEvent;

    [Header("References")]
    [SerializeField] Image highlightImage;
    [SerializeField] TextMeshProUGUI titleDisplay;
    [SerializeField] TextMeshProUGUI scoreDisplay;

    [SerializeField] UnityEvent OnSelectEvent;

    bool interactable;

    public void SetInteractableState (bool value)
    {
        if (titleDisplay)
        {
            titleDisplay.color = value ? highlightColor : disabledColor;
            if (!value)
                titleDisplay.text = "???";
        }

        if (scoreDisplay)
            scoreDisplay.color = value ? highlightColor : disabledColor;

        interactable = value;
    }

    public void SetHighlightState (bool value)
    {
        if (highlightImage)
            highlightImage.enabled = value;
    }

    public void OnSubmit()
    {
        if (submitAKEvent != null)
            submitAKEvent.Post(gameObject);

        if (!interactable)
            return;

        if (OnSelectEvent != null)
            OnSelectEvent.Invoke();
    }
}
