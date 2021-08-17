using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextMeshProUGUI dialogDisplay;

    bool showing;

    void Start()
    {
        showing = false;
    }

    public void SetDialog (string name, string dialog)
    {
        gameObject.SetActive(true);

        if (nameDisplay)
            nameDisplay.text = name;

        if (dialogDisplay)
            dialogDisplay.text = dialog;
    }

    public void EndDialog()
    {
        gameObject.SetActive(false);

        if (nameDisplay)
            nameDisplay.text = string.Empty;

        if (dialogDisplay)
            dialogDisplay.text = string.Empty;
    }
}
