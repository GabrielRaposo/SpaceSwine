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
    [SerializeField] GameObject exclamationIcon;

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

    public void SetScore (int value)
    {
        if (!scoreDisplay)
            return;

        scoreDisplay.text = value < 0 ? "---" : value.ToString();
    }

    public void SetExclamationIcon (bool value)
    {
        if (exclamationIcon == null)
            return;

        exclamationIcon.SetActive(value);
    }
}
