using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DialogBox : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    [Header("References")]
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextMeshProUGUI dialogDisplay;

    bool showing;
    CanvasGroup canvasGroup;

    Sequence sequence;

    public void SetDialog (string name, string dialog)
    {
        if (!showing)
        {
            gameObject.SetActive(true);

            transform.localScale = new Vector3 (1, 0);
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            if (sequence != null)
                sequence.Kill();

            sequence.Append( transform.DOScaleY ( 1f, transitionDuration ) );
            sequence.Join ( canvasGroup.DOFade ( 1f, transitionDuration ) );
        }
        showing = true;

        if (nameDisplay)
            nameDisplay.text = name;

        if (dialogDisplay)
            dialogDisplay.text = dialog;
    }

    public void EndDialog()
    {
        if (nameDisplay)
            nameDisplay.text = string.Empty;

        if (dialogDisplay)
            dialogDisplay.text = string.Empty;
        
        transform.localScale = new Vector3 (1, 1);
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
            
        if (sequence != null)
            sequence.Kill();

        sequence.Append( transform.DOScaleY ( 0f, transitionDuration ) );
        sequence.Join ( canvasGroup.DOFade ( 0f, transitionDuration ) );

        showing = false;
    }

    private void OnDisable() 
    {
        showing = false;    
    }
}
