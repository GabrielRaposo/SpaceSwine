using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RedBlueGames.Tools.TextTyper;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DialogBox : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    [Header("References")]
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextTyper dialogTyper;

    bool showing;
    CanvasGroup canvasGroup;

    int dialogIndex;
    string speakerName;
    List <string> dialogs;

    Sequence sequence;

    public void SetDialogData(string speakerName, List<string> dialogs)
    {
        this.speakerName = speakerName;
        this.dialogs = dialogs;

        dialogIndex = 0;
        SetDialog(speakerName, dialogs[dialogIndex]);

        DialogSystem.OnDialog = true;
    }

    private void Update() 
    {
        if (!DialogSystem.OnDialog)
            return;

        if (Input.GetButtonDown("Jump"))
            ForwardInput();
    }

    private void ForwardInput()
    {
        if (dialogTyper.IsTyping)
        {
            dialogTyper.Skip();
        } 
        else
        {
            if (++dialogIndex < dialogs.Count)
            {
                SetDialog( speakerName, dialogs[dialogIndex] );
            }
            else
            {
                EndDialog();
                DialogSystem.OnDialog = false;
            }
        }
    }

    public void SetDialog (string name, string dialog)
    {
        if (!showing)
        {
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

        if (dialogTyper)
            dialogTyper.TypeText (dialog);
    }

    public void EndDialog()
    {
        if (nameDisplay)
            nameDisplay.text = string.Empty;

        if (dialogTyper)
            dialogTyper.TypeText (string.Empty);
        
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
