using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PagerConfirmationScreen : MonoBehaviour
{
    [SerializeField] GameObject descriptionTab;
    [SerializeField] TextMeshProUGUI titleDisplay;
    [SerializeField] TextMeshProUGUI descriptionDisplay;

    UnityAction ConfirmEvent;
    UnityAction CancelEvent;

    public void SetScreen(string title, string description, UnityAction ConfirmEvent, UnityAction CancelEvent)
    {
        if (titleDisplay)
            titleDisplay.text = title;

        if (descriptionDisplay && descriptionTab)
        {
            descriptionTab.SetActive(description.Length > 0);
            descriptionDisplay.text = description;
        }

        this.ConfirmEvent = ConfirmEvent;
        this.CancelEvent = CancelEvent;
    }

    public void ConfirmInput()
    {
        if (ConfirmEvent != null)
            ConfirmEvent.Invoke();
    }

    public void CancelInput()
    {
        if (CancelEvent != null)
            CancelEvent.Invoke();
    }
}
